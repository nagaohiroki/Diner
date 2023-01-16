﻿/*
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CostData
{
	[SerializeField]
	CardData.CardType mCostType;
	[SerializeField]
	int mNum;
	public CardData.CardType GetCostType => mCostType;
	public int GetNum => mNum;
}
[System.Serializable]
public class CardDataCounter
{
	[SerializeField]
	CardData mCardData;
	[SerializeField]
	int mNum;
	public CardData GetCardData => mCardData;
	public int GetNum => mNum;
}
[System.Serializable]
public class DeckData
{
	[SerializeField]
	List<CardDataCounter> mCardDataCouneter;
	public List<CardData> GenerateCardList()
	{
		var list = new List<CardData>();
		foreach(var card in mCardDataCouneter)
		{
			for(int i = 0; i < card.GetNum; i++)
			{
				list.Add(card.GetCardData);
			}
		}
		return list;
	}
}
public class GameData : MonoBehaviour
{
	[SerializeField]
	int winPoint;
	[SerializeField]
	List<DeckData> deckList;
	public List<DeckData> GetDeckList => deckList;
	public int GetWinPoint => winPoint;
}
*/
