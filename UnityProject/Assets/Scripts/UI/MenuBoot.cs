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
	public void Save()
	{
		var user = SaveData.instance.userData;
		var option = SaveData.instance.optionData;
		user.name = mUserName.text;
		user.imageColorCode = imageColorCode;
		option.isLocal = mLocalMode.isOn;
		SaveData.instance.Save();
	}
	public void Load()
	{
		var user = SaveData.instance.userData;
		var option = SaveData.instance.optionData;
		mUserName.text = user.name;
		mLocalMode.isOn = option.isLocal;
		imageColorCode = user.imageColorCode;
		mImage.color = UserData.IntToColor(imageColorCode);
	}
	void Start()
	{
		Load();
	}
}
