using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : NetworkBehaviour
{
	PlayerInput mInput;
	GameController mGameController;
	public override void OnNetworkSpawn()
	{
		Rename();
		if(IsOwner)
		{
			mGameController = FindObjectOfType<GameController>();
			mInput = FindObjectOfType<PlayerInput>();
		}
	}
	void Rename()
	{
		var newName = $"Player{OwnerClientId}";
		if(IsOwner)
		{
			newName += $"_Owner";
		}
		if(IsOwnedByServer)
		{
			newName += $"_Host";
		}
		name = newName;
		Debug.Log($"login:{newName}");
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
		if(IsOwner)
		{
			Move();
			if(mInput.actions["Fire"].triggered)
			{
				mGameController.Turn();
			}
		}
	}
}
