using UnityEngine;
using Unity.Netcode;
using MemoryPack;
using System.Collections.Generic;
public class NetworkSelector : MonoBehaviour
{
	[SerializeField]
	MenuRoot mMenuRoot;
	public Dictionary<ulong, byte[]> connectionsData { get; private set; } = new Dictionary<ulong, byte[]>();
	public void StartLocalHost()
	{
		SetupUser();
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		NetworkManager.Singleton.StartHost();
		mMenuRoot.SwitchMenu<MenuQuit>().SetActiveHostButton(true);
	}
	public void StartLocalClient()
	{
		SetupUser();
		NetworkManager.Singleton.StartClient();
		mMenuRoot.SwitchMenu<MenuQuit>().SetActiveHostButton(false);
	}
	public void StartHost()
	{
		SetupUser();
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		mMenuRoot.SwitchMenu<MenuLoading>();
		StartCoroutine(RelaySetting.StartHost(5, (result, code) =>
		{
			if(result && NetworkManager.Singleton.StartHost())
			{
				var quit = mMenuRoot.SwitchMenu<MenuQuit>();
				quit.password = code;
				quit.SetActiveHostButton(true);
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
				mMenuRoot.SwitchMenu<MenuQuit>().SetActiveHostButton(false);
				return;
			}
			mMenuRoot.CreateDialog(2.0f, () => mMenuRoot.SwitchMenu<MenuJoin>()).AddText("Error!!");
		}));
	}
	public void StartServer()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
		NetworkManager.Singleton.StartServer();
	}
	public void Logout()
	{
		NetworkManager.Singleton.Shutdown();
		Debug.Log("Logout");
	}
	void SetupUser()
	{
		connectionsData.Clear();
		var data = new ConnectionData();
		var menuBoot = mMenuRoot.GetComponentInChildren<MenuBoot>(true);
		menuBoot.Save();
		data.user = menuBoot.userData;
		NetworkManager.Singleton.NetworkConfig.ConnectionData = MemoryPackSerializer.Serialize<ConnectionData>(data);
		Debug.Log($"SetupUser:{data}");
	}
	void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		response.Approved = true;
		response.CreatePlayerObject = true;
		response.PlayerPrefabHash = null;
		response.Position = Vector3.zero;
		response.Rotation = Quaternion.identity;
		response.Pending = false;
		connectionsData.Add(request.ClientNetworkId, request.Payload);
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
