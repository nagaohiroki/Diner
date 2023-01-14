using UnityEngine;
using Unity.Netcode;
using TMPro;
using MemoryPack;
public class MenuQuit : MonoBehaviour
{
	[SerializeField]
	GameObject mGameStart;
	[SerializeField]
	GameObject mPassword;
	const string mDefaultText = "Password";
	public void TogglePassword(TextMeshProUGUI inText)
	{
		if(inText.text == mDefaultText)
		{
			var connectionData = MemoryPackSerializer.Deserialize<ConnectionData>(NetworkManager.Singleton.NetworkConfig.ConnectionData);
			inText.text = connectionData.password;
			return;
		}
		inText.text = mDefaultText;
	}
	void OnEnable()
	{
		bool isServer = NetworkManager.Singleton.IsServer;
		mGameStart.SetActive(isServer);
		mPassword.SetActive(isServer);
	}
}
