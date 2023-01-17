using UnityEngine;
using System.Collections.Generic;
public class Table : MonoBehaviour
{
	[SerializeField]
	MenuRoot mMenuRoot;
	[SerializeField]
	List<DeckModel> mDeckModels;
	public void Apply(GameInfo inGameInfo)
	{
		foreach(var deck in mDeckModels)
		{
			deck.Apply(inGameInfo);
		}
		var winner = inGameInfo.GetWinner();
		if(winner != null)
		{
			mMenuRoot.Result(winner);
		}
	}
	public void Clear()
	{
		foreach(var deck in mDeckModels)
		{
			deck.Clear();
		}
	}
}
