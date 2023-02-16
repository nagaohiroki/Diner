using MemoryPack;
using UnityEngine;
using System.IO;
using UnityUtility;
public static class SaveUtility
{
	public static T Load<T>(string inPath) where T : class
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
	public static void Save<T>(string inPath, T inData)
	{
		var path = MakePath(inPath);
		var bytes = MemoryPackSerializer.Serialize<T>(inData);
		using(var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
		{
			stream.Write(bytes, 0, bytes.Length);
		}
	}
	static string MakePath(string inPath)
	{
		var path = inPath;
#if UNITY_EDITOR
		path = Path.GetFileName(Path.GetFullPath(Path.Join(Application.dataPath, ".."))) + "_" + path;
#endif
		return Path.Join(Application.persistentDataPath, path) + ".dat";
	}
}
[MemoryPackable]
public partial class UserData
{
	public string name { get; set; }
	public string id { get; set; }
	public int imageColorCode { get; set; }
	public Color imageColor => IntToColor(imageColorCode);
	public static string fileName => "savedata";
	public static UserData NewSaveData => new UserData
	{
		name = $"user#{RandomObject.GetGlobal.Range(0, 10000).ToString("D4")}",
		id = $"user_{System.Guid.NewGuid().ToString()}",
		imageColorCode = GenerateRandomColor()
	};
	public override string ToString()
	{
		return $"user:{name}, id:{id}, imageColor:{imageColor}";
	}
	public static int GenerateRandomColor()
	{
		return RandomObject.GetGlobal.Range(0, System.Convert.ToInt32("ffffff", 16));
	}
	public static Color IntToColor(int inCode)
	{
		if(ColorUtility.TryParseHtmlString($"#{inCode.ToString("X")}", out var color))
		{
			return color;
		}
		return Color.white;
	}
}
[MemoryPackable]
public partial class ConnectionData
{
	public UserData user { get; set; }
	public RuleData rule { get; set; }
	public int botLevel { get; set; }
	public override string ToString()
	{
		return $"{user}\n{botLevel}\n{rule}";
	}
}
[MemoryPackable]
public partial class OptionData
{
	public static string fileName => "option";
	public bool isLocal { get; set; }
}
[MemoryPackable]
public partial class RuleData
{
	public static string fileName => "rule";
	public bool isBonus { get; set; } = true;
	public bool isCoin { get; set; } = true;
	public int MemberNum { get; set; } = 5;
	public override string ToString()
	{
		return $"isBonus:{isBonus}, isCoin:{isCoin}, MemberNum:{MemberNum}";
	}

}
