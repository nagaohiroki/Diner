using UnityEngine;
using Unity.Netcode;
using TMPro;
public class MenuQuit : MonoBehaviour
{
	[SerializeField]
	GameObject mGameStart;
	[SerializeField]
	GameObject mPassword;
	const string mDefaultText = "Password";
	public string password{private get;set;}
	public void TogglePassword(TextMeshProUGUI inText)
	{
		if(inText.text == mDefaultText)
		{
			inText.text = password;
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
