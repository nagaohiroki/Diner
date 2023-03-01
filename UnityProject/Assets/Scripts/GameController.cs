using Unity.Netcode;
using UnityEngine;
using UnityUtility;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using MemoryPack;
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
	[SerializeField]
	float mTweenSpeed = 0.3f;
	int mSeed;
	Dictionary<string, Player> mEntryPlayers;
	public Vector3 GetMoveRange => mMoveRange;
	public BattleData GetData => mData;
	public GameInfo gameInfo { get; set; }
	public bool isStart => gameInfo != null;
	public PlayerInput GetInput => mInput;
	public int entryPlayerNum => mEntryPlayers != null ? mEntryPlayers.Count : 0;
	public RuleData ruleData { get; set; }
	int mRestartCounter;
	Dictionary<ulong, byte[]> connectionsData { get; set; } = new Dictionary<ulong, byte[]>();
	public bool IsTurnPlayer(Player inPlayer)
	{
		return gameInfo != null && gameInfo.GetCurrentTurnPlayer == inPlayer.id;
	}
	public byte[] FindUserData(ulong inClientId, ulong inNetworkObjectId)
	{
		if(inNetworkObjectId == NetworkManager.Singleton.ConnectedClients[inClientId].PlayerObject.NetworkObjectId)
		{
			return connectionsData[inClientId];
		}
		return null;
	}
	public override void OnNetworkDespawn()
	{
		Clear();
		base.OnNetworkDespawn();
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
	public void DisconnectClient(ulong inId)
	{
		if(!IsServer || !isStart)
		{
			return;
		}
		var data = connectionsData[inId];
		var connection = MemoryPackSerializer.Deserialize<ConnectionData>(data);
		var obj = NetworkManager.Singleton.ConnectedClients[inId];
		Debug.Log($"DisconnectClient: {inId}:{connection.user.id}:{obj.PlayerObject.transform.position}");
		CreateBot(1, connection.user.id, obj.PlayerObject.transform.position);
	}
	public void GameStart()
	{
		if(IsServer && !isStart)
		{
			var players = FindObjectsOfType<Player>();
			if(players.Length <= mPlayerChairs.maxNum)
			{
				mMenuRoot.GetComponentInChildren<MenuQuit>().SetActiveHostButton(false);
				GameStartClientRpc(null, RandomObject.GenerateSeed());
			}
		}
	}
	public void Clear()
	{
		NetworkManager.Singleton.Shutdown();
		mMenuRoot.SwitchMenu<MenuBoot>();
		gameInfo = null;
		mTable.Clear();
		connectionsData.Clear();
		mRestartCounter = 0;
	}
	public Player GetPlayer(string inId)
	{
		return mEntryPlayers.TryGetValue(inId, out var player) ? player : null;
	}
	public void AddBot()
	{
		CreateBot(1, null, Vector3.zero);
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
	public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		if(isStart)
		{
			var data = MemoryPackSerializer.Deserialize<ConnectionData>(request.Payload);
			if(!mEntryPlayers.ContainsKey(data.user.id))
			{
				response.Approved = false;
				return;
			}
		}
		response.Approved = true;
		response.CreatePlayerObject = true;
		response.PlayerPrefabHash = null;
		response.Position = RandomPos();
		response.Rotation = Quaternion.identity;
		response.Pending = false;
		connectionsData.Add(request.ClientNetworkId, request.Payload);
	}
	public void Restart(Player inPlayer, int inServerEntryPlayerNum)
	{
		ReplacePlayer(inPlayer);
		RestartGame(inServerEntryPlayerNum);
	}
	void RestartGame(int inServerEntryPlayerNum)
	{
		if(isStart || IsServer)
		{
			return;
		}
		++mRestartCounter;
		if(mRestartCounter == inServerEntryPlayerNum)
		{
			GameRestartServerRpc();
		}
	}
	void ReplacePlayer(Player inPlayer)
	{
		if(!isStart)
		{
			return;
		}
		var oldPlayer = mEntryPlayers[inPlayer.id];
		if(oldPlayer.NetworkObject.IsSpawned)
		{
			oldPlayer.NetworkObject.Despawn(true);
		}
		mEntryPlayers[inPlayer.id] = inPlayer;
		mPlayerChairs.Sitdown(gameInfo.GetPlayerInfos, mEntryPlayers);
	}
	Vector3 RandomPos()
	{
		var x = RandomObject.GetGlobal.Range(-mMoveRange.x, mMoveRange.x);
		var z = RandomObject.GetGlobal.Range(-mMoveRange.z, mMoveRange.z);
		return new Vector3(x, 0.0f, z);
	}
	Player CreateBot(int inNpcLevel, string inId, Vector3 inPos)
	{
		var players = FindObjectsOfType<Player>();
		if(players.Length >= mPlayerChairs.maxNum)
		{
			return null;
		}
		var pos = inId == null ? RandomPos() : inPos;
		var go = Instantiate(NetworkManager.Singleton.NetworkConfig.PlayerPrefab, pos, Quaternion.identity);
		if(!go.TryGetComponent<Player>(out var player))
		{
			return null;
		}
		player.reserveId = inId;
		player.botLevel = inNpcLevel;
		player.NetworkObject.Spawn();
		return player;
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
	void GameStartInternal(byte[] inPickData, int inSeed)
	{
		if(isStart)
		{
			return;
		}
		mSeed = inSeed;
		mEntryPlayers = EntryPlayers();
		gameInfo = new GameInfo();
		gameInfo.GameStart(mData, inSeed, mEntryPlayers, inPickData, ruleData);
		mPlayerChairs.Sitdown(gameInfo.GetPlayerInfos, mEntryPlayers);
		mTable.Apply(this, mTweenSpeed);
		Debug.Log($"seed:{inSeed}");
	}
	[ServerRpc(RequireOwnership = false)]
	void PickServerRpc(int inDeck, int inCard)
	{
		PickClientRpc(inDeck, inCard);
	}
	[ClientRpc]
	void PickClientRpc(int inDeck, int inCard)
	{
		if(gameInfo != null)
		{
			gameInfo.Pick(inDeck, inCard);
			mTable.Apply(this, mTweenSpeed);
		}
	}
	[ServerRpc(RequireOwnership = false)]
	void GameRestartServerRpc()
	{
		if(isStart)
		{
			GameStartClientRpc(MemoryPackSerializer.Serialize(gameInfo.pickInfo), mSeed);
		}
	}
	[ClientRpc]
	void GameStartClientRpc(byte[] inPickData, int inSeed)
	{
		GameStartInternal(inPickData, inSeed);
	}
}
