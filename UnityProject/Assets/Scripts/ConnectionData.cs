using MemoryPack;
using UnityEngine;
using System.IO;
using UnityUtility;
[MemoryPackable]
public partial class UserData
{
	public string name { get; set; }
	public string id { get; set; }
	public int imageColorCode { get; set; }
	public Color imageColor => IntToColor(imageColorCode);
	static string userPath => Path.Join(Application.persistentDataPath, "savedata.dat");
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
	public static UserData NewSaveData()
	{
		return new UserData
		{
			name = $"user#{RandomObject.GetGlobal.Range(0, 10000).ToString("D4")}",
			id = $"user_{System.Guid.NewGuid().ToString()}",
			imageColorCode = GenerateRandomColor()
		};
	}
	public static UserData Load()
	{
		if(!File.Exists(userPath))
		{
			return null;
		}
		using(var stream = new FileStream(userPath, FileMode.Open, FileAccess.Read))
		{
			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			return MemoryPackSerializer.Deserialize<UserData>(bytes);
		}
	}
	public static void Save(UserData inUserData)
	{
		var bytes = MemoryPackSerializer.Serialize<UserData>(inUserData);
		using(var stream = new FileStream(userPath, FileMode.OpenOrCreate, FileAccess.Write))
		{
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
[MemoryPackable]
public partial class ConnectionData
{
	public UserData user { get; set; }
	public override string ToString()
	{
		return $"{user}";
	}
}
