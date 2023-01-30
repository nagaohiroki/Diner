using Unity.Netcode;
using UnityEngine;
using UnityUtility;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class GameController : NetworkBehaviour
{
	[SerializeField]
	Table mTable;
	[SerializeField]
	PlayerChairs mPlayerChairs;
	[SerializeField]
	BattleData mData;
	[SerializeField]
	MenuRoot mMenuRoot;
	[SerializeField]
	PlayerInput mInput;
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	Dictionary<string, Player> mEntryPlayers;
	public BattleData GetData => mData;
	public GameInfo gameInfo { get; set; }
	public bool isStart => gameInfo != null;
	public Player GetCurrentTurnPlayer => GetPlayer(gameInfo.GetCurrentTurnPlayer);
	public PlayerInput GetInput => mInput;
	public bool IsTurnPlayer(Player inPlayer)
	{
		return gameInfo != null && gameInfo.GetCurrentTurnPlayer == inPlayer.id;
	}
	public void Pick(int inDeck, int inCard)
	{
		if(!gameInfo.CanPick(inDeck, inCard))
		{
			return;
		}
		if (mTable.IsTween)
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
		base.OnNetworkSpawn();
	}
	public override void OnNetworkDespawn()
	{
		Clear();
		base.OnNetworkDespawn();
	}
	public void GameStart()
	{
		if(IsServer && !isStart)
		{
			GameStartClientRpc();
		}
	}
	public void Clear()
	{
		NetworkManager.Singleton.Shutdown();
		mMenuRoot.SwitchMenu<MenuBoot>();
		gameInfo = null;
		mTable.Clear();
		if(IsServer)
		{
			randomSeed.Value = RandomObject.GenerateSeed();
		}
	}
	public Player GetPlayer(string inId)
	{
		if(mEntryPlayers.TryGetValue(inId, out var player))
		{
			return player;
		}
		return null;
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
			mTable.Apply(this);
		}
	}
	[ClientRpc]
	void GameStartClientRpc()
	{
		var players = FindObjectsOfType<Player>();
		mEntryPlayers = new Dictionary<string, Player>();
		gameInfo = new GameInfo();
		foreach(var player in players)
		{
			if(!mEntryPlayers.ContainsKey(player.id))
			{
				mEntryPlayers.Add(player.id, player);
			}
		}
		gameInfo.GameStart(mData, randomSeed.Value, mEntryPlayers, mPlayerChairs);
		mTable.Apply(this);
		Debug.Log($"seed:{randomSeed.Value}");
	}
}
