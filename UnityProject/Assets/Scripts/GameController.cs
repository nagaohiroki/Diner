using Unity.Netcode;
using UnityEngine;
using UnityUtility;
using System.Collections.Generic;
public class GameController : NetworkBehaviour
{
	NetworkVariable<int> turnPlayer = new NetworkVariable<int>();
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	RandomObject mTurn;
	Player[] mPlayers;
	public bool IsGameStart => mPlayers != null;
	public void Turn()
	{
		TurnChangeServerRpc();
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
	public bool IsTurnPlayer(int inIndex)
	{
		return inIndex == turnPlayer.Value;
	}
	int IndexFromClientId(ulong inId, List<ulong> inIndex)
	{
		for(int i = 0; i < inIndex.Count; i++)
		{
			if(inIndex[i] == inId)
			{
				return i;
			}
		}
		return -1;
	}
	[ClientRpc()]
	void GameStartClientRpc()
	{
		if(mPlayers != null)
		{
			return;
		}
		var players = FindObjectsOfType<Player>();
		if(players.Length < 1)
		{
			return;
		}
		var turn = new List<ulong>();
		foreach(var player in players)
		{
			turn.Add(player.OwnerClientId);
		}
		mTurn = new RandomObject(randomSeed.Value);
		mTurn.Shuffle(turn);
		foreach(var player in players)
		{
			player.turnIndex = IndexFromClientId(player.OwnerClientId, turn);
		}
		mPlayers = players;
		var str = "GameStart\n";
		foreach(var player in players)
		{
			str += $"player:{player.OwnerClientId}, turn:{player.turnIndex}\n";
		}
		Debug.Log(str);
	}
	[ServerRpc(RequireOwnership = false)]
	void TurnChangeServerRpc()
	{
		Increment();
	}
	void Increment()
	{
		if(mPlayers == null)
		{
			return;
		}
		turnPlayer.Value += 1;
		if(turnPlayer.Value >= mPlayers.Length)
		{
			turnPlayer.Value = 0;
		}
		Debug.Log($"Turn:{turnPlayer.Value}");
	}
}
