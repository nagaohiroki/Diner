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
		var players = FindObjectsOfType<Player>();
		gameInfo.GameStart(randomSeed.Value, mTable.DeckNum, players, Vector3.zero, 4.0f);
		if(gameInfo.IsStart)
		{
			mTable.Apply(gameInfo);
		}
	}
	[ServerRpc(RequireOwnership = false)]
	void PickServerRpc(int inDeck, int inCard)
	{
		PickClientRpc(inDeck, inCard);
	}
	[ClientRpc]
	void PickClientRpc(int inDeck, int inCard)
	{
		if(gameInfo.IsStart)
		{
			gameInfo.Pick(inDeck, inCard);
			mTable.Apply(gameInfo);
		}
	}
}
