﻿using Unity.Netcode;
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
	void Apply(byte[] inData)
	{
		if(id != null)
		{
			return;
		}
		var data = MemoryPackSerializer.Deserialize<ConnectionData>(inData);
		var user = data.user;
		id = user.id;
		botLevel = data.botLevel;
		var render = mModel.GetComponent<Renderer>();
		render.material.color = user.imageColor;
		mCache = render.material;
		mName.color = user.imageColor;
		name = user.name;
		mName.text = user.name;
	}
	[ServerRpc(RequireOwnership = false)]
	void SpawnServerRpc(ulong inClientId, ulong inNetworkObjectId)
	{
		if(botLevel > 0)
		{
			SpawnClientRpc(CreateBotData(botLevel));
			return;
		}
		if(inNetworkObjectId == NetworkManager.Singleton.ConnectedClients[inClientId].PlayerObject.NetworkObjectId)
		{
			var dataList = NetworkManager.Singleton.GetComponent<NetworkSelector>().connectionsData;
			SpawnClientRpc(dataList[inClientId]);
		}
	}
	[ClientRpc]
	void SpawnClientRpc(byte[] inData)
	{
		Apply(inData);
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
		mGameController.Pick(card.deckIndex, card.cardIndex);
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
		mGameController.Pick(pick.deck, pick.card);
	}
	byte[] CreateBotData(int inBotLevel)
	{
		var user = new UserData
		{
			name = $"bot#Lv{inBotLevel}",
			id = $"bot_{System.Guid.NewGuid().ToString()}",
			imageColorCode = System.Convert.ToInt32(ColorUtility.ToHtmlStringRGB(Color.blue), 16)
		};
		var data = new ConnectionData
		{
			botLevel = inBotLevel,
			user = user
		};
		return MemoryPackSerializer.Serialize(data);
	}
}
