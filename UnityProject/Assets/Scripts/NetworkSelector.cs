using UnityEngine;
using Unity.Netcode;
using MemoryPack;
public class NetworkSelector : MonoBehaviour
{
	[SerializeField]
	MenuRoot mMenuRoot;
	public void StartHost()
	{
		SetupUser();
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		mMenuRoot.SwitchMenu<MenuLoading>();
		StartCoroutine(RelaySetting.StartHost(5, (result, code) =>
		{
			if(result && NetworkManager.Singleton.StartHost())
			{
				mMenuRoot.SwitchMenu<MenuQuit>().password = code;
				return;
			}
			mMenuRoot.CreateDialog(2.0f, () => mMenuRoot.SwitchMenu<MenuCreate>()).AddText("Error!!");
		}));
	}
	public void StartClient(MenuJoin inJoin)
	{
		SetupUser();
		var menu = mMenuRoot.GetComponentInChildren<MenuJoin>(true);
		mMenuRoot.SwitchMenu<MenuLoading>();
		StartCoroutine(RelaySetting.StartClient(menu.GetPassword, result =>
		{
			if(result && NetworkManager.Singleton.StartClient())
			{
				mMenuRoot.SwitchMenu<MenuQuit>();
				return;
			}
			mMenuRoot.CreateDialog(2.0f, () => mMenuRoot.SwitchMenu<MenuJoin>()).AddText("Error!!");
		}));
	}
	public void StartServer()
	{
		NetworkManager.Singleton.StartServer();
	}
	public void Logout()
	{
		NetworkManager.Singleton.Shutdown();
		Debug.Log("Logout");
	}
	void SetupUser()
	{
		var data = new ConnectionData();
		var menuBoot = mMenuRoot.GetComponentInChildren<MenuBoot>(true);
		menuBoot.Save();
		data.user = menuBoot.userData;
		NetworkManager.Singleton.NetworkConfig.ConnectionData = MemoryPackSerializer.Serialize<ConnectionData>(data);
		Debug.Log($"SetupUser:{data}");
	}
	void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		var payload = MemoryPackSerializer.Deserialize<ConnectionData>(request.Payload);
		var myData = MemoryPackSerializer.Deserialize<ConnectionData>(NetworkManager.Singleton.NetworkConfig.ConnectionData);
		response.Approved = true;
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
		mMenuRoot.SwitchMenu<MenuBoot>().Load();
#endif
	}
}
