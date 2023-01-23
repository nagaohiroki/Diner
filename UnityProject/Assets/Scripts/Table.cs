﻿using UnityEngine;
using System.Collections.Generic;
public class Table : MonoBehaviour
{
	[SerializeField]
	GameObject mTurnClock;
	[SerializeField]
	MenuRoot mMenuRoot;
	[SerializeField]
	List<DeckModel> mDeckModels;
	public void Apply(GameController inGameController)
	{
		foreach(var deck in mDeckModels)
		{
			deck.Apply(inGameController);
		}
		var winner = inGameController.gameInfo.GetWinner();
		if(winner != null)
		{
			mMenuRoot.Result(inGameController.GetPlayer(winner));
		}
		LeanTween.delayedCall(1.0f, () =>
		{
			var trans = inGameController.GetCurrentTurnPlayer.transform;
			LeanTween.move(mTurnClock, trans.position + trans.rotation * Vector3.forward * 2.0f, 0.2f);
		});
	}
	public void Clear()
	{
		foreach(var deck in mDeckModels)
		{
			deck.Clear();
		}
	}
}
