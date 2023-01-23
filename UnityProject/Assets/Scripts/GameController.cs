using Unity.Netcode;
using UnityEngine;
using UnityUtility;
using MemoryPack;
using System.Collections.Generic;
public class GameController : NetworkBehaviour
{
	[SerializeField]
	Table mTable;
	[SerializeField]
	PlayerChairs mPlayerChairs;
	[SerializeField]
	BattleData mData;
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	public BattleData GetData => mData;
	public GameInfo gameInfo { get; set; }
	UserList mUserList;
	Dictionary<string, Player> mPlayers;
	public bool isStart => gameInfo != null;
	public Player GetCurrentTurnPlayer => mPlayers[gameInfo.GetCurrentTurnPlayer];
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
		PickServerRpc(inDeck, inCard);
	}
	public override void OnNetworkSpawn()
	{
		if(IsServer)
		{
			randomSeed.Value = RandomObject.GenerateSeed();
		}
		mUserList = new UserList();
		var cd = MemoryPackSerializer.Deserialize<ConnectionData>(NetworkManager.Singleton.NetworkConfig.ConnectionData);
		AddUserServerRpc(NetworkManager.LocalClientId, MemoryPackSerializer.Serialize(cd.user));
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
		gameInfo = null;
		mTable.Clear();
		if(IsServer)
		{
			randomSeed.Value = RandomObject.GenerateSeed();
		}
	}
	public Player GetPlayer(string inId)
	{
		return mPlayers[inId];
	}
	[ServerRpc(RequireOwnership = false)]
	void AddUserServerRpc(ulong inId, byte[] inUserData)
	{
		var data = MemoryPack.MemoryPackSerializer.Deserialize<UserData>(inUserData);
		ApplyPlayer(inId, data);
		mUserList.Add(inId, data);
		foreach(var user in mUserList.userList)
		{
			AddUserClientRpc(user.Key, MemoryPackSerializer.Serialize(user.Value));
		}

		Debug.Log(mUserList);
	}
	[ClientRpc]
	void AddUserClientRpc(ulong inId, byte[] inUserData)
	{
		if(!IsServer)
		{
			var data = MemoryPack.MemoryPackSerializer.Deserialize<UserData>(inUserData);
			mUserList.Add(inId, data);
			ApplyPlayer(inId, data);
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
			mTable.Apply(this);
		}
	}
	[ClientRpc]
	void GameStartClientRpc()
	{
		var players = FindObjectsOfType<Player>();
		mPlayers = new Dictionary<string, Player>();
		gameInfo = new GameInfo();
		foreach(var player in players)
		{
			player.id = mUserList.userList[player.OwnerClientId].id;
			mPlayers.Add(player.id, player);
		}
		gameInfo.GameStart(mData, randomSeed.Value, mPlayers, mPlayerChairs);
		mTable.Apply(this);
		Debug.Log($"seed:{randomSeed.Value}");
	}
	void ApplyPlayer(ulong inId, UserData inUserData)
	{
		var players = FindObjectsOfType<Player>();
		foreach(var player in players)
		{
			if(player.OwnerClientId == inId)
			{
				player.Apply(inUserData);
			}
		}
	}
}
