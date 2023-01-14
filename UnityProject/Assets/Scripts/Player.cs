using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : NetworkBehaviour
{
	[SerializeField]
	LayerMask mFocusLayerMask;
	PlayerInput mInput;
	GameController mGameController;
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
		var v = mInput.actions["Move"].ReadValue<Vector2>() * Time.deltaTime * 10.0f;
		if(v == Vector2.zero)
		{
			return;
		}
		var move = new Vector3(v.x, 0.0f, v.y);
		transform.position += move;
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
		if(!IsOwner)
		{
			return;
		}
		if(!mGameController.isStart)
		{
			Move();
			return;
		}
		if(mGameController.turnPlayer == this)
		{
			if(mInput.actions["Fire"].triggered)
			{
				Pick();
			}
		}
	}
}
