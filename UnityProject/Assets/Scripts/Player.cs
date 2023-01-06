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
		if(Physics.Raycast(ray, out var hit, Mathf.Infinity, mFocusLayerMask))
		{
			Debug.Log($"{cursor}:{hit.collider.name}");
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
		if(!IsOwner)
		{
			return;
		}
		Pick();
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(!mGameController.gameInfo.IsStart)
			{
				mGameController.GameStart();
				return;
			}
			if(mGameController.gameInfo.IsTurn(OwnerClientId))
			{
				mGameController.Pick(0, 0);
			}
			else
			{
				Debug.Log("あなたのターンではない");
			}
		}
	}
}
