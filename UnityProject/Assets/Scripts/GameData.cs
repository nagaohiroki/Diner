﻿using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CostData
{
	public enum CostType
	{
		// 料理
		Cooking,
		// 肉
		Meat,
		// 麺
		Noodles,
		// 海鮮
		Sea,
		// 野菜
		Vegetable,
		// 乳製品
		Milk,
		// ソース
		Sause,
		// スープ
		Soup,
		// 芋
		Potato,
		// 合計
		Num
	}
	[SerializeField]
	CostData.CostType mCostType;
	[SerializeField]
	int mNum;
	public CostType GetCostType => mCostType;
	public int GetNum => mNum;
	public static CostData Dummy()
	{
		return new CostData
		{
			mCostType = CostType.Meat,
			mNum = 2,
		};
	}
}
[System.Serializable]
public class CardData
{
	[SerializeField]
	string id;
	[SerializeField]
	CostData.CostType costType;
	[SerializeField]
	List<CostData> cost;
	public string GetId => id;
	public List<CostData> GetCost => cost;
	public CostData.CostType GetCostType => costType;
	public override string ToString()
	{
		return id;
	}
	public int GetCostNum(CostData.CostType inCostType)
	{
		foreach(var c in cost)
		{
			if(c.GetCostType == inCostType)
			{
				return c.GetNum;
			}
		}
		return 0;
	}
	public static CardData FoodDummy()
	{
		return new CardData
		{
			id = "Meat",
			costType = CostData.CostType.Meat
		};
	}
	public static CardData CookDummy()
	{
		return new CardData
		{
			id = "Sukiyaki",
			costType = CostData.CostType.Cooking,
			cost = new List<CostData> { CostData.Dummy() }
		};
	}
}
public class GameData : MonoBehaviour
{
	[SerializeField]
	List<CardData> cardList;
	public List<CardData> GetCardList => cardList;
	public CardData GetCardData(string inId)
	{
		foreach(var card in cardList)
		{
			if(card.GetId == inId)
			{
				return card;
			}
		}
		return null;
	}
	[ContextMenu("Dummy")]
	public void Dummy()
	{
		cardList.Clear();
		for(int i = 0; i < 50; i++)
		{
			cardList.Add(CardData.CookDummy());
		}
		for(int i = 0; i < 50; i++)
		{
			cardList.Add(CardData.FoodDummy());
		}
	}
}
