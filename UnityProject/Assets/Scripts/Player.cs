using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : NetworkBehaviour
{
	PlayerInput mController;
	public override void OnNetworkSpawn()
	{
		var owner = IsOwner ? ":Owner" : "";
		var host = IsHost ? ":Host" : "";
		var newName = $"Player:{OwnerClientId}{owner}{host}";
		name = newName;
		if(IsOwner)
		{
			mController = FindObjectOfType<PlayerInput>();
		}
		Debug.Log($"login:{newName}");
	}
}
