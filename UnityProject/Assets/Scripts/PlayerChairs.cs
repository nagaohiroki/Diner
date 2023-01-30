using UnityEngine;
using System.Collections.Generic;
public class PlayerChairs : MonoBehaviour
{
	enum Dir
	{
		Down,
		DownLeft,
		Left,
		UpLeft,
		Up,
		UpRight,
		Right,
		DownRight
	}
	[SerializeField]
	Vector3 mOffset;
	readonly List<List<Dir>> mEntryChair = new List<List<Dir>>
	{
		new List<Dir>{Dir.Down},
		new List<Dir>{Dir.Down, Dir.Up},
		new List<Dir>{Dir.Down, Dir.Left, Dir.Right},
		new List<Dir>{Dir.Down, Dir.Left, Dir.Up, Dir.Right},
		new List<Dir>{Dir.Down, Dir.Left, Dir.UpLeft, Dir.UpRight, Dir.Right},
	};
	readonly Dictionary<Dir, Vector3> mOffsets = new Dictionary<Dir, Vector3>
	{
		{Dir.Down,      new Vector3( 0.0f, 0.0f, -1.0f)},
		{Dir.DownLeft,  new Vector3(-1.0f, 0.0f, -1.0f)},
		{Dir.Left,      new Vector3(-1.0f, 0.0f,  0.0f)},
		{Dir.UpLeft,    new Vector3(-1.0f, 0.0f,  1.0f)},
		{Dir.Up,        new Vector3( 0.0f, 0.0f,  1.0f)},
		{Dir.UpRight,   new Vector3( 1.0f, 0.0f,  1.0f)},
		{Dir.Right,     new Vector3( 1.0f, 0.0f,  0.0f)},
		{Dir.DownRight, new Vector3( 1.0f, 0.0f, -1.0f)},
	};
	public void Sitdown(List<string> inTurn, Dictionary<string, Player> inPlayers)
	{
		int ownerIndex = OwnerPlayerIndex(inTurn, inPlayers);
		var directions = GetDirections(inTurn.Count);
		float baseAngle = 360.0f / (float)mOffsets.Count;
		for(int i = 0; i < inTurn.Count; ++i)
		{
			var dir = directions[i];
			int ownerBaseIndex = (ownerIndex + i) % inTurn.Count;
			var player = inPlayers[inTurn[ownerBaseIndex]];
			player.transform.position = Vector3.Scale(mOffsets[dir], mOffset); ;
			player.rot = baseAngle * (int)dir;
		}
	}
	List<Dir> GetDirections(int inNum)
	{
		foreach(var chair in mEntryChair)
		{
			if(chair.Count == inNum)
			{
				return chair;
			}
		}
		return null;
	}
	int OwnerPlayerIndex(List<string> inTurn, Dictionary<string, Player> inPlayers)
	{
		for(int i = 0; i < inTurn.Count; ++i)
		{
			var player = inPlayers[inTurn[i]];
			if(player != null && player.IsOwner && !player.isBot)
			{
				return i;
			}
		}
		return -1;
	}
}
