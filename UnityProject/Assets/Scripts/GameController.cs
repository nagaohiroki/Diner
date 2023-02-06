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
	[SerializeField]
	Vector3 mMoveRange;
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	Dictionary<string, Player> mEntryPlayers;
	public Vector3 GetMoveRange => mMoveRange;
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
		if(gameInfo.GetWinners(mData.GetWinPoint) != null)
		{
			return;
		}
		if(!gameInfo.CanPick(inDeck, inCard))
		{
			return;
		}
		if(mTable.IsTween)
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
			var players = FindObjectsOfType<Player>();
			if(players.Length <= mPlayerChairs.maxNum)
			{
				mMenuRoot.GetComponentInChildren<MenuQuit>().SetActiveHostButton(false);
				GameStartClientRpc();
			}
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
	public void AddBot(int inNpcLevel)
	{
		if(isStart)
		{
			return;
		}
		var players = FindObjectsOfType<Player>();
		if(players.Length >= mPlayerChairs.maxNum)
		{
			return;
		}
		var x = RandomObject.GetGlobal.Range(-mMoveRange.x, mMoveRange.x);
		var z = RandomObject.GetGlobal.Range(-mMoveRange.z, mMoveRange.z);
		var go = Instantiate(NetworkManager.Singleton.NetworkConfig.PlayerPrefab, new Vector3(x, 0.0f, z), Quaternion.identity);
		if(go.TryGetComponent<Player>(out var player))
		{
			player.botLevel = inNpcLevel;
			player.NetworkObject.Spawn();
		}
	}
	public void RemoveBot()
	{
		var players = FindObjectsOfType<Player>();
		foreach(var player in players)
		{
			if(player.isBot)
			{
				Destroy(player.gameObject);
				break;
			}
		}
	}
	Dictionary<string, Player> EntryPlayers()
	{
		var players = FindObjectsOfType<Player>();
		var entryPlayers = new Dictionary<string, Player>();
		foreach(var player in players)
		{
			if(!entryPlayers.ContainsKey(player.id))
			{
				entryPlayers.Add(player.id, player);
			}
		}
		return entryPlayers;
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
		mEntryPlayers = EntryPlayers();
		gameInfo = new GameInfo();
		gameInfo.GameStart(mData, randomSeed.Value, mEntryPlayers, mPlayerChairs);
		mTable.Apply(this);
		Debug.Log($"seed:{randomSeed.Value}");
	}
}
