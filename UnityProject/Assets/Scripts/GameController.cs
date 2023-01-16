using Unity.Netcode;
using UnityEngine;
using UnityUtility;
using MemoryPack;
public class GameController : NetworkBehaviour
{
	[SerializeField]
	Table mTable;
	[SerializeField]
	BattleData mData;
	NetworkVariable<int> randomSeed = new NetworkVariable<int>();
	GameInfo gameInfo { get; set; }
	UserList mUserList;
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
			mTable.Apply(gameInfo);
		}
	}
	[ClientRpc]
	void GameStartClientRpc()
	{
		var players = FindObjectsOfType<Player>();
		gameInfo = new GameInfo();
		gameInfo.GameStart(mData, randomSeed.Value, players, Vector3.zero, 4.0f);
		mTable.Apply(gameInfo);
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
