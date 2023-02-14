﻿using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Cost
{
	[SerializeField]
	CardData.CardType mCostType;
	[SerializeField]
	int mNum;
	public CardData.CardType GetCostType => mCostType;
	public int GetNum => mNum;
	public override string ToString()
	{
		return $"{mCostType}x{mNum}";
	}
}
[System.Serializable]
public class BonusCost
{
	[SerializeField]
	CardData.BonusType mCookType;
	[SerializeField]
	int mNum;
	public CardData.BonusType GetCookType => mCookType;
	public int GetNum => mNum;
	public override string ToString()
	{
		return $"{mCookType}x{mNum}";
	}
}
[CreateAssetMenu]
public class CardData : ScriptableObject
{
	public enum CardType
	{
		Cooking,
		Meat,
		SeaFood,
		Vegetable,
		Milk,
		Spices,
		Grain,
		Rare,
		Coin
	}
	public enum BonusType
	{
		First,
		Main,
		Sub,
		Dessert,
	}
	[SerializeField]
	string mName;
	[SerializeField]
	string id;
	[SerializeField]
	BonusType bonusType;
	[SerializeField]
	CardType cardType;
	[SerializeField]
	int point;
	[SerializeField]
	int money;
	[SerializeField]
	List<Cost> cost;
	[SerializeField]
	List<BonusCost> bonus;
	public string GetId => id;
	public int GetPoint => point;
	public int GetMoney => money;
	public List<Cost> GetCost => cost;
	public List<BonusCost> GetBonusCosts => bonus;
	public CardType GetCardType => cardType;
	public override string ToString()
	{
		var text = $"{id}";
		if(cost.Count != 0)
		{
			var costLog = string.Empty;
			foreach(var c in cost)
			{
				costLog += $"{c}, ";
			}
			text += $" (point:{point} money:{money} cost:{costLog})";
		}
		return text;
	}
	public int GetCostNum(CardType inCostType)
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
	void OnValidate()
	{
		id = name.Replace("CardFood", "").Replace("CardCook", "");
	}
}
