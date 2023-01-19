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
	PlayerInput mInput;
	GameController mGameController;
	public string id { get; set; }
	public float rot { get; set; }
	public override void OnNetworkSpawn()
	{
		if(IsOwner)
		{
			mGameController = FindObjectOfType<GameController>();
			mInput = FindObjectOfType<PlayerInput>();
		}
	}
	public void Apply(UserData inUserData)
	{
		name = inUserData.name;
		var render = GetComponentInChildren<Renderer>();
		render.material.color = inUserData.imageColor;
		mName.text = name;
		mName.color = inUserData.imageColor;
	}
	[ServerRpc]
	void MoveServerRpc(Vector3 inPos)
	{
		transform.position = inPos;
	}
	[ClientRpc]
	void MoveClientRpc(Vector3 inPos)
	{
		if(!IsServer)
		{
			transform.position = inPos;
		}
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
		transform.position += new Vector3(v.x, 0.0f, v.y) * Time.deltaTime * 5.0f;
		if(!IsServer)
		{
			MoveServerRpc(transform.position);
		}
		else
		{
			MoveClientRpc(transform.position);
		}
	}
	void Update()
	{
		if(IsOwner)
		{
			Move();
			Pick();
		}
	}
}
