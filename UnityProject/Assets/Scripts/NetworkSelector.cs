using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Text;
public class NetworkSelector : MonoBehaviour
{
	public void StartHost(TextMeshProUGUI inPassword)
	{
		NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(inPassword.text);
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		NetworkManager.Singleton.StartHost();
		Debug.Log($"StartHost:{inPassword.text}");
	}
	public void StartClient(TMP_InputField inPassword)
	{
		NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(inPassword.text);
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
		var pass = Encoding.ASCII.GetString(request.Payload);
		var reqPass = Encoding.ASCII.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
		response.Approved = pass == reqPass;
		response.CreatePlayerObject = true;
		response.PlayerPrefabHash = null;
		response.Position = Vector3.zero;
		response.Rotation = Quaternion.identity;
		response.Pending = false;
		Debug.Log($"request:{request.ClientNetworkId} Payload:{pass} isApproved:{response.Approved}");
	}
	void Awake()
	{
#if UNITY_SERVER
		StartServer();
#endif
	}
}
