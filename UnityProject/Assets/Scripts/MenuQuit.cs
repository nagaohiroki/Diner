using UnityEngine;
using Unity.Netcode;
public class MenuQuit : MonoBehaviour
{
	[SerializeField]
	GameObject mGameStart;
	[SerializeField]
	GameObject mPassword;
	[SerializeField]
	GameController mGameController;
	void OnEnable()
	{
		bool isServer = NetworkManager.Singleton.IsServer;
		mGameStart.SetActive(isServer);
		mPassword.SetActive(isServer);
	}
}
