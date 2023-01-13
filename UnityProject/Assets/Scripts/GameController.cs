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
	GameInfo gameInfo { get; set; }
	public bool isStart => gameInfo != null;
	public Player turnPlayer => gameInfo.GetCurrentTurnPlayer;
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
	[ClientRpc]
	void GameStartClientRpc()
	{
		gameInfo = new GameInfo();
		gameInfo.GameStart(mGameData, randomSeed.Value, FindObjectsOfType<Player>(), Vector3.zero, 4.0f);
		mTable.Apply(gameInfo);
		Debug.Log($"seed:{randomSeed.Value}");
	}
}
