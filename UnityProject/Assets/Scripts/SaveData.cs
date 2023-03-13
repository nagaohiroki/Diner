using UnityEngine;
public class SaveData : MonoBehaviour
{
	public static SaveData instance { get; private set; }
	public OptionData optionData { get; set; }
	public UserData userData { get; set; }
	public void Save()
	{
		SaveUtility.Save(UserData.fileName, userData);
		SaveUtility.Save(OptionData.fileName, optionData);
	}
	public void Load()
	{
		userData = SaveUtility.Load<UserData>(UserData.fileName);
		if(userData == null)
		{
			userData = UserData.NewSaveData;
		}
		optionData = SaveUtility.Load<OptionData>(OptionData.fileName);
		if(optionData == null)
		{
			optionData = new OptionData();
		}
	}
	void Awake()
	{
		instance = this;
		Load();
	}
}
