using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuBoot : MonoBehaviour
{
	[SerializeField]
	TMP_InputField mUserName;
	[SerializeField]
	Image mImage;
	int imageColorCode;
	public void ChangeColor()
	{
		imageColorCode = UserData.GenerateRandomColor();
		mImage.color = UserData.IntToColor(imageColorCode);
	}
	public void Save(UserData inSaveData)
	{
		if(IsChange(inSaveData))
		{
			inSaveData.name = mUserName.text;
			inSaveData.imageColorCode = imageColorCode;
			UserData.Save(inSaveData);
		}
	}
	public UserData Load()
	{
		var userData = UserData.Load();
		if(userData == null)
		{
			userData = UserData.NewSaveData();
		}
		mUserName.text = userData.name;
		imageColorCode = userData.imageColorCode;
		mImage.color = UserData.IntToColor(imageColorCode);
		return userData;
	}
	bool IsChange(UserData inSaveData)
	{
		return mUserName.text != inSaveData.name || imageColorCode != inSaveData.imageColorCode;
	}
}
