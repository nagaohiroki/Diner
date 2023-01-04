using Unity.Netcode;
using UnityEngine;
public class GameController : NetworkBehaviour
{
	public NetworkVariable<int> turnPlayer = new NetworkVariable<int>();
	public override void OnNetworkSpawn()
	{
		turnPlayer.OnValueChanged += OnValueChanged;
	}
	public override void OnNetworkDespawn()
	{
		turnPlayer.OnValueChanged -= OnValueChanged;
	}
	void OnValueChanged(int inPre, int inCurrent)
	{
		Debug.Log($"turnPlayer:{inCurrent}");
	}
	public void Turn()
	{
		if(IsServer)
		{
			Increment();
			return;
		}
		TurnChangeServerRpc();
	}
	[ServerRpc(RequireOwnership = false)]
	void TurnChangeServerRpc()
	{
		Increment();
	}
	void Increment()
	{
		turnPlayer.Value += 1;
	}
}
