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
	public void SetActiveHostButton(bool inActive)
	{
		mGameStart.SetActive(inActive);
		mPassword.SetActive(inActive);
	}
	void OnEnable()
	{
		SetActiveHostButton(NetworkManager.Singleton.IsServer);
	}
}
