using Unity.Netcode;
using UnityEngine;
using UnityUtility;
public class GameController : NetworkBehaviour
{
	[SerializeField]
	Table mTable;
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	public GameInfo gameInfo { get; set; } = new GameInfo();
	public void Pick(int inDeck, int inCard)
	{
		PickServerRpc(inDeck, inCard);
	}
	public override void OnNetworkSpawn()
	{
		if(IsServer)
		{
			randomSeed.Value = RandomObject.GenerateSeed();
		}
		Debug.Log($"seed:{randomSeed.Value}");
	}
	public void GameStart()
	{
		if(IsServer)
		{
			GameStartClientRpc();
		}
	}
	[ClientRpc]
	void GameStartClientRpc()
	{
		gameInfo.GameStart(randomSeed.Value, FindObjectsOfType<Player>());
		mTable.Apply(gameInfo);
	}
	[ServerRpc(RequireOwnership = false)]
	void PickServerRpc(int inCard, int inDeck)
	{
		PickClientRpc(inCard, inDeck);
	}
	[ClientRpc]
	void PickClientRpc(int inCard, int inDeck)
	{
		gameInfo.Pick(inCard, inDeck);
	}
}
