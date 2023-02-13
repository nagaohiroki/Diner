using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuBoot : MonoBehaviour
{
	[SerializeField]
	TMP_InputField mUserName;
	[SerializeField]
	Image mImage;
	[SerializeField]
	Toggle mLocalMode;
	int imageColorCode;
	public void ChangeColor()
	{
		imageColorCode = UserData.GenerateRandomColor();
		mImage.color = UserData.IntToColor(imageColorCode);
	}
	public void Save(UserData inUserData, OptionData inOptionData)
	{
		inUserData.name = mUserName.text;
		inUserData.imageColorCode = imageColorCode;
		inOptionData.isLocal = mLocalMode.isOn;
	}
	public void Load(UserData inUserData, OptionData inOptionData)
	{
		mUserName.text = inUserData.name;
		imageColorCode = inUserData.imageColorCode;
		mImage.color = UserData.IntToColor(imageColorCode);
		mLocalMode.isOn = inOptionData.isLocal;
	}
}
