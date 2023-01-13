using System.Collections.Generic;
using UnityEngine;
public class UserList
{
	Dictionary<ulong, UserData> userList { get; set; } = new Dictionary<ulong, UserData>();
	public override string ToString()
	{
		var log = "user\n";
		foreach(var id in userList)
		{
			log += $"{id.Key}:{id.Value}\n";
		}
		return log;
	}
	public void Add(ulong inId, UserData inUser)
	{
		userList[inId] = inUser;
	}
	public void Remove(ulong inId)
	{
		userList.Remove(inId);
		Debug.Log($"ClientDisconnect{inId}");
	}
}
