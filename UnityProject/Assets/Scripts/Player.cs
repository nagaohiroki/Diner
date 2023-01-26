using Unity.Netcode;
using UnityEngine;
using TMPro;
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
	public string id { get; private set; }
	public bool isNPC { get; set; }
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
		if(IsOwner && !isNPC)
		{
			mGameController = FindObjectOfType<GameController>();
		}
	}
	public void Apply(UserData inUserData)
	{
		id = inUserData.id;
		name = inUserData.name;
		var render = mModel.GetComponent<Renderer>();
		render.material.color = inUserData.imageColor;
		mCache = render.material;
		mName.text = inUserData.name;
		mName.color = inUserData.imageColor;
	}
	[ServerRpc]
	void MoveServerRpc(Vector3 inPos)
	{
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
		mPos += new Vector3(v.x, 0.0f, v.y) * Time.deltaTime * 5.0f;
		MoveServerRpc(mPos);
	}
	void Update()
	{
		if(IsOwner && !isNPC)
		{
			Move();
			Pick();
			if(Input.GetKeyDown(KeyCode.Return))
			{
				EntryNpc();
			}
		}
	}
	void EntryNpc()
	{
		var go = Instantiate(NetworkManager.Singleton.NetworkConfig.PlayerPrefab);
		if(go.TryGetComponent<Player>(out var player))
		{
			player.NetworkObject.Spawn();
			player.isNPC = true;
			player.Apply(UserData.NewSaveData());
		}
	}
	public override void OnDestroy()
	{
		if(mCache != null)
		{
			Destroy(mCache);
		}
		base.OnDestroy();
	}
}
