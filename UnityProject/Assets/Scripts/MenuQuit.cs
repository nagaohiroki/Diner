using UnityEngine;
using TMPro;
public class MenuQuit : MonoBehaviour
{
	[SerializeField]
	GameObject[] mHostButtons;
	[SerializeField]
	GameObject mPassword;
	const string mDefaultText = "Password";
	public string password { private get; set; }
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
		foreach(var button in mHostButtons)
		{
			button.SetActive(inActive);
		}
	}
}
