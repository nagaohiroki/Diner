using Unity.Netcode;
using UnityEngine;
using UnityUtility;
public class GameController : NetworkBehaviour
{
	[SerializeField]
	Table mTable;
	[SerializeField]
	GameData mGameData;
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	public GameInfo gameInfo { get; private set; }
	public void Pick(int inDeck, int inCard)
	{
		if(!gameInfo.CanPick(inDeck, inCard))
		{
			return;
		}
		PickServerRpc(inDeck, inCard);
	}
	public override void OnNetworkSpawn()
	{
		if(IsServer)
		{
			randomSeed.Value = RandomObject.GenerateSeed();
		}
	}
	public void GameStart()
	{
		if(IsServer)
		{
			GameStartClientRpc();
		}
	}
	public void Clear()
	{
		gameInfo = null;
		mTable.Clear();
		if(IsServer)
		{
			randomSeed.Value = RandomObject.GenerateSeed();
		}
	}
	[ClientRpc]
	void GameStartClientRpc()
	{
		gameInfo = new GameInfo();
		gameInfo.GameStart(mGameData, randomSeed.Value, FindObjectsOfType<Player>(), Vector3.zero, 4.0f);
		mTable.Apply(gameInfo);
		Debug.Log($"seed:{randomSeed.Value}");
	}
	[ServerRpc(RequireOwnership = false)]
	void PickServerRpc(int inDeck, int inCard)
	{
		PickClientRpc(inDeck, inCard);
	}
	[ClientRpc]
	void PickClientRpc(int inDeck, int inCard)
	{
		if(gameInfo != null && gameInfo.IsStart)
		{
			gameInfo.Pick(inDeck, inCard);
			mTable.Apply(gameInfo);
		}
	}
}
