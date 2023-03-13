using MemoryPack;
using UnityEngine;
using UnityUtility;
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
		if(ColorUtility.TryParseHtmlString($"#{inCode.ToString("X6")}", out var color))
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
	public string language { get; set; } = "ja";
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
