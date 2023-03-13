using UnityEngine;
using System.IO;
using MemoryPack;
public class SaveData : MonoBehaviour
{
	public static SaveData instance { get; private set; }
	public OptionData optionData { get; private set; }
	public UserData userData { get; private set; }
	public override string ToString()
	{
		return $"{optionData}\n{userData}";
	}
	public void Save()
	{
		SaveFile(UserData.fileName, userData);
		SaveFile(OptionData.fileName, optionData);
	}
	public void Load()
	{
		userData = LoadFile<UserData>(UserData.fileName);
		if(userData == null)
		{
			userData = UserData.NewSaveData;
		}
		optionData = LoadFile<OptionData>(OptionData.fileName);
		if(optionData == null)
		{
			optionData = new OptionData();
		}
	}
	T LoadFile<T>(string inPath) where T : class
	{
		var path = MakePath(inPath);
		if(!File.Exists(path))
		{
			return null;
		}
		using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			return MemoryPackSerializer.Deserialize<T>(bytes);
		}
	}
	void SaveFile<T>(string inPath, T inData)
	{
		var path = MakePath(inPath);
		var bytes = MemoryPackSerializer.Serialize<T>(inData);
		using(var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
		{
			stream.Write(bytes, 0, bytes.Length);
		}
	}
	string MakePath(string inPath)
	{
		var path = inPath;
#if UNITY_EDITOR
		path = Path.GetFileName(Path.GetFullPath(Path.Join(Application.dataPath, ".."))) + "_" + path;
#endif
		return Path.Join(Application.persistentDataPath, path) + ".dat";
	}
	void Awake()
	{
		instance = this;
		Load();
	}
}
