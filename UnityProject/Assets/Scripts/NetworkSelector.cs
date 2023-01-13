using UnityEngine;
using Unity.Netcode;
using MemoryPack;
public class NetworkSelector : MonoBehaviour
{
	[SerializeField]
	MenuBoot mMenuBoot;
	public void StartHost(MenuCreate inCreate)
	{
		SetupUser(inCreate.CreateConnectData());
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		NetworkManager.Singleton.StartHost();
	}
	public void StartClient(MenuJoin inJoin)
	{
		SetupUser(inJoin.CreateConnectData());
		NetworkManager.Singleton.StartClient();
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
	void SetupUser(ConnectionData inData)
	{
		mMenuBoot.Save();
		inData.user = mMenuBoot.userData;
		NetworkManager.Singleton.NetworkConfig.ConnectionData = MemoryPackSerializer.Serialize<ConnectionData>(inData);
		Debug.Log($"SetupUser:{inData}");
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
	}
	void Awake()
	{
#if UNITY_SERVER
		StartServer();
#else
		mMenuBoot.Load();
#endif
	}
}
