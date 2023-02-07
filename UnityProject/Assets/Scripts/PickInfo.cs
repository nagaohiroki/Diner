using MemoryPack;
using System.Collections.Generic;
[MemoryPackable]
public partial class PickInfo
{
	public int deck { get; set; }
	public int card { get; set; }
	public override string ToString()
	{
		return $"Pick deck:{deck}, card{card}";
	}
}
[MemoryPackable]
public partial class PickInfoList
{
	public List<PickInfo> picks { get; set; }
	public int Count => picks.Count;
	public void Add(PickInfo inPick)
	{
		picks.Add(inPick);
	}
	public PickInfo Get(int inIndex)
	{
		return picks[inIndex];
	}
	public static PickInfoList Load(byte[] inData)
	{
		if(inData == null)
		{
			return new PickInfoList { picks = new List<PickInfo>() };
		}
		return MemoryPackSerializer.Deserialize<PickInfoList>(inData);
	}
}
