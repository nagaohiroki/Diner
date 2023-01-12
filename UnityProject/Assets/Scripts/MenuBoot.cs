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
	public UserData userData { get; private set; }
	public void ChangeColor()
	{
		imageColorCode = UserData.GenerateRandomColor();
		mImage.color = UserData.IntToColor(imageColorCode);
	}
	public void Save()
	{
		if(IsChange(userData))
		{
			userData.name = mUserName.text;
			userData.imageColorCode = imageColorCode;
			UserData.Save(userData);
		}
	}
	public void Load()
	{
		userData = UserData.Load();
		if(userData == null)
		{
			userData = UserData.NewSaveData();
		}
		mUserName.text = userData.name;
		imageColorCode = userData.imageColorCode;
		mImage.color = UserData.IntToColor(imageColorCode);
	}
	bool IsChange(UserData inSaveData)
	{
		return mUserName.text != inSaveData.name || imageColorCode != inSaveData.imageColorCode;
	}
}
