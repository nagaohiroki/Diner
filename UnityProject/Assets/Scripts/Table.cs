using UnityEngine;
using System.Collections.Generic;
public class Table : MonoBehaviour
{
	[SerializeField]
	MenuRoot mMenuRoot;
	[SerializeField]
	List<DeckModel> mDeckModels;
	public void Apply(GameInfo inGameInfo, GameController inGameController)
	{
		foreach(var deck in mDeckModels)
		{
			deck.Apply(inGameInfo, inGameController);
		}
		var winner = inGameInfo.GetWinner();
		if(winner != null)
		{
			mMenuRoot.Result(inGameController.GetPlayer(winner));
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
