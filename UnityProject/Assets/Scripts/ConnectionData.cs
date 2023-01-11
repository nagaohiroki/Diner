using MemoryPack;
[MemoryPackable]
public partial class ConnectionData
{
	public string user { get; set; }
	public string password { get; set; }
	public override string ToString()
	{
		return $"user:{user} , password:{password}";
	}
}
