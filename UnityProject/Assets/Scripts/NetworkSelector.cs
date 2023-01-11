using UnityEngine;
using TMPro;
using Unity.Netcode;
using MemoryPack;
public class NetworkSelector : MonoBehaviour
{
	public void StartHost(TextMeshProUGUI inPassword)
	{
		var data = new ConnectionData { password = inPassword.text };
		var bytes = MemoryPackSerializer.Serialize<ConnectionData>(data);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = bytes;
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		NetworkManager.Singleton.StartHost();
		Debug.Log($"StartHost:{inPassword.text}");
	}
	public void StartClient(TMP_InputField inPassword)
	{
		var data = new ConnectionData { password = inPassword.text };
		var bytes = MemoryPackSerializer.Serialize<ConnectionData>(data);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = bytes;
		NetworkManager.Singleton.StartClient();
		Debug.Log($"StartClient:{inPassword.text}");
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
		Debug.Log($"request:{request.ClientNetworkId} Payload:{payload} isApproved:{response.Approved}");
	}
	void Awake()
	{
#if UNITY_SERVER
		StartServer();
#endif
	}
}
