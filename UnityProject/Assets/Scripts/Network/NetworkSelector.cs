using UnityEngine;
using Unity.Netcode;
using MemoryPack;
public class NetworkSelector : MonoBehaviour
{
	[SerializeField]
	GameController mGameContorller;
	[SerializeField]
	MenuRoot mMenuRoot;
	public void StartHost()
	{
		SetupUser(true);
		if(SaveData.instance.optionData.isLocal)
		{
			NetworkManager.Singleton.StartHost();
			mMenuRoot.SwitchMenu<MenuQuit>().SetActiveHostButton(true);
			return;
		}
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
		SetupUser(false);
		if(SaveData.instance.optionData.isLocal)
		{
			NetworkManager.Singleton.StartClient();
			mMenuRoot.SwitchMenu<MenuQuit>().SetActiveHostButton(false);
			return;
		}
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
		NetworkManager.Singleton.StartServer();
	}
	void SetupUser(bool inIsHost)
	{
		var data = new ConnectionData();
		var menuBoot = mMenuRoot.GetComponentInChildren<MenuBoot>(true);
		if(inIsHost)
		{
			var menuCreate = mMenuRoot.GetComponentInChildren<MenuCreate>(true);
			data.rule = menuCreate.GetRule;
		}
		menuBoot.Save();
		data.user = SaveData.instance.userData;
		NetworkManager.Singleton.NetworkConfig.ConnectionData = MemoryPackSerializer.Serialize<ConnectionData>(data);
		Debug.Log($"SetupUser:{data}");
	}
	void Awake()
	{
		NetworkManager.Singleton.OnClientDisconnectCallback += mGameContorller.DisconnectClient;
		NetworkManager.Singleton.ConnectionApprovalCallback = mGameContorller.ApprovalCheck;
#if UNITY_SERVER
		StartServer();
#else
		mMenuRoot.SwitchMenu<MenuBoot>();
#endif
	}
}
