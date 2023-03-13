using Unity.Netcode;
using UnityEngine;
using TMPro;
using MemoryPack;
public class Player : NetworkBehaviour
{
	[SerializeField]
	LayerMask mFocusLayerMask;
	[SerializeField]
	TextMeshPro mName;
	[SerializeField]
	GameObject mModel;
	GameController mGameController;
	Vector3 mPos;
	Material mCache;
	public int botLevel { private get; set; }
	public string id { get; set; }
	public string reserveId { get; set; }
	public bool isBot => botLevel > 0;
	public float rot
	{
		get
		{
			return mModel.transform.eulerAngles.y;
		}
		set
		{
			mModel.transform.rotation = Quaternion.Euler(0.0f, value, 0.0f);
		}
	}
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		mGameController = FindObjectOfType<GameController>();
		mPos = transform.position;
		SpawnServerRpc(OwnerClientId, NetworkObjectId);
	}
	public override void OnDestroy()
	{
		if(mCache != null)
		{
			Destroy(mCache);
		}
		base.OnDestroy();
	}
	void Apply(byte[] inData, int inServerEntryPlayerNum)
	{
		var data = MemoryPackSerializer.Deserialize<ConnectionData>(inData);
		var user = data.user;
		if(data.rule != null)
		{
			mGameController.ruleData = data.rule;
		}
		if(id == user.id)
		{
			return;
		}
		id = user.id;
		botLevel = data.botLevel;
		var render = mModel.GetComponent<Renderer>();
		render.material.color = user.imageColor;
		mCache = render.material;
		name = user.name;
		mName.text = user.name;
		mGameController.Restart(this, inServerEntryPlayerNum);
	}
	[ServerRpc(RequireOwnership = false)]
	void SpawnServerRpc(ulong inClientId, ulong inNetworkObjectId)
	{
		if(botLevel > 0)
		{
			SpawnClientRpc(CreateBotData(), mGameController.entryPlayerNum);
			return;
		}
		var data = mGameController.FindUserData(inClientId, inNetworkObjectId);
		if(data != null)
		{
			SpawnClientRpc(data, mGameController.entryPlayerNum);
		}
	}
	[ClientRpc]
	void SpawnClientRpc(byte[] inData, int inServerEntryPlayerNum)
	{
		Apply(inData, inServerEntryPlayerNum);
	}
	[ServerRpc]
	void MoveServerRpc(Vector3 inPos)
	{
		if(mGameController != null && mGameController.isStart)
		{
			return;
		}
		MoveClientRpc(inPos);
	}
	[ClientRpc]
	void MoveClientRpc(Vector3 inPos)
	{
		transform.position = inPos;
	}
	void Pick()
	{
		if(!mGameController.IsTurnPlayer(this))
		{
			return;
		}
		var input = mGameController.GetInput;
		if(!input.actions["Fire"].triggered)
		{
			return;
		}
		var cursor = input.actions["Cursor"].ReadValue<Vector2>();
		var ray = Camera.main.ScreenPointToRay(cursor);
		if(!Physics.Raycast(ray, out var hit, Mathf.Infinity, mFocusLayerMask))
		{
			return;
		}
		if(!hit.collider.TryGetComponent<CardModel>(out var card))
		{
			return;
		}
		var info = card.cardInfo;
		if(!mGameController.gameInfo.CanPick(info))
		{
			card.CanNotPick();
			return;
		}
		mGameController.Pick(info);
	}
	void Move()
	{
		if(mGameController.isStart)
		{
			return;
		}
		var v = mGameController.GetInput.actions["Move"].ReadValue<Vector2>();
		if(v == Vector2.zero)
		{
			return;
		}
		var range = mGameController.GetMoveRange;
		mPos += new Vector3(v.x, 0.0f, v.y) * Time.deltaTime * 5.0f;
		mPos.x = Mathf.Clamp(mPos.x, -range.x, range.x);
		mPos.z = Mathf.Clamp(mPos.z, -range.z, range.z);
		MoveServerRpc(mPos);
	}
	void Update()
	{
		TurnMotion();
		if(IsOwner && !isBot)
		{
			Move();
			Pick();
			return;
		}
		if(isBot && IsServer)
		{
			BotPick();
		}
	}
	void TurnMotion()
	{
		if(mGameController == null)
		{
			return;
		}
		if(!mGameController.IsTurnPlayer(this))
		{
			if(LeanTween.isTweening(mModel))
			{
				LeanTween.cancel(mModel);
				LeanTween.moveLocal(mModel, new Vector3(0.0f, 0.1f, 0.0f), 0.3f);
			}
			return;
		}
		if(!LeanTween.isTweening(mModel))
		{
			LeanTween.moveLocal(mModel, new Vector3(0.0f, 0.5f, 0.0f), 0.3f).setLoopPingPong();
		}
	}
	void BotPick()
	{
		if(!mGameController.IsTurnPlayer(this))
		{
			return;
		}
		var pick = mGameController.gameInfo.AIPick();
		if(pick == null)
		{
			return;
		}
		mGameController.Pick(pick);
	}
	byte[] CreateBotData()
	{
		var newId = id;
		if(newId == null)
		{
			newId = reserveId != null ? reserveId : $"bot_{System.Guid.NewGuid().ToString()}";
		}
		var user = new UserData
		{
			name = $"bot#Lv{botLevel}",
			id = newId,
			imageColorCode = System.Convert.ToInt32(ColorUtility.ToHtmlStringRGB(Color.blue), 16)
		};
		var data = new ConnectionData
		{
			botLevel = botLevel,
			user = user
		};
		return MemoryPackSerializer.Serialize(data);
	}
}
