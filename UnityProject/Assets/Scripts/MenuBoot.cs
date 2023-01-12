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
		imageColorCode = SaveData.GenerateRandomColor();
		mImage.color = SaveData.IntToColor(imageColorCode);
	}
	public void Save(SaveData inSaveData)
	{
		if(IsChange(inSaveData))
		{
			inSaveData.name = mUserName.text;
			inSaveData.imageColorCode = imageColorCode;
			SaveData.Save(inSaveData);
		}
	}
	public SaveData Load()
	{
		var saveData = SaveData.Load();
		if(saveData == null)
		{
			saveData = SaveData.NewSaveData();
		}
		mUserName.text = saveData.name;
		imageColorCode = saveData.imageColorCode;
		return saveData;
	}
	bool IsChange(SaveData inSaveData)
	{
		return mUserName.text != inSaveData.name || imageColorCode != inSaveData.imageColorCode;
	}
}
