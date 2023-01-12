using UnityEngine;
using Unity.Netcode;
using MemoryPack;
public class NetworkSelector : MonoBehaviour
{
	[SerializeField]
	MenuBoot mMenuBoot;
	SaveData saveData;
	public void StartHost(MenuCreate inCreate)
	{
		mMenuBoot.Save(saveData);
		var data = inCreate.CreateConnectData();
		data.save = saveData;
		var bytes = MemoryPackSerializer.Serialize<ConnectionData>(data);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = bytes;
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		NetworkManager.Singleton.StartHost();
		Debug.Log($"StartHost:{data}");
	}
	public void StartClient(MenuJoin inJoin)
	{
		mMenuBoot.Save(saveData);
		var data = inJoin.CreateConnectData();
		data.save = saveData;
		var bytes = MemoryPackSerializer.Serialize<ConnectionData>(data);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = bytes;
		NetworkManager.Singleton.StartClient();
		Debug.Log($"StartClient:{data}");
	}
	public void StartServer()
	{
		NetworkManager.Singleton.StartServer();
		Debug.Log("StartServer");
	}
	public void Logout()
	{
		NetworkManager.Singleton.Shutdown();
		Debug.Log("Logout");
	}
	void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		var payload = MemoryPackSerializer.Deserialize<ConnectionData>(request.Payload);
		var myData = MemoryPackSerializer.Deserialize<ConnectionData>(NetworkManager.Singleton.NetworkConfig.ConnectionData);
		response.Approved = payload.password == myData.password;
		response.CreatePlayerObject = true;
		response.PlayerPrefabHash = null;
		response.Position = Vector3.zero;
		response.Rotation = Quaternion.identity;
		response.Pending = false;
		Debug.Log($"request:{request.ClientNetworkId}\nPayload:{payload}\nisApproved:{response.Approved}");
	}
	void Awake()
	{
#if UNITY_SERVER
		StartServer();
#else
		saveData = mMenuBoot.Load();
#endif
	}
}
