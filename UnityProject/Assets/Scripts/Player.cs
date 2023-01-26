using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class Player : NetworkBehaviour
{
	[SerializeField]
	LayerMask mFocusLayerMask;
	[SerializeField]
	TextMeshPro mName;
	[SerializeField]
	GameObject mModel;
	PlayerInput mInput;
	GameController mGameController;
	Vector3 mPos;
	Material mCache;
	public string id { get; set; }
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
		if(IsOwner)
		{
			mGameController = FindObjectOfType<GameController>();
			mInput = FindObjectOfType<PlayerInput>();
		}
		base.OnNetworkSpawn();
	}
	public void Apply(UserData inUserData)
	{
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
		if(!mInput.actions["Fire"].triggered)
		{
			return;
		}
		var cursor = mInput.actions["Cursor"].ReadValue<Vector2>();
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
		var v = mInput.actions["Move"].ReadValue<Vector2>();
		if(v == Vector2.zero)
		{
			return;
		}
		mPos += new Vector3(v.x, 0.0f, v.y) * Time.deltaTime * 5.0f;
		MoveServerRpc(mPos);
	}
	void Update()
	{
		if(IsOwner)
		{
			Move();
			Pick();
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
