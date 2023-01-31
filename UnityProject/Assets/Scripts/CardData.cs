using System.Collections.Generic;
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
		Grain
	}
	[SerializeField]
	string id;
	[SerializeField]
	CardType cardType;
	[SerializeField]
	int point;
	[SerializeField]
	List<Cost> cost;
	public string GetId => id;
	public int GetPoint => point;
	public List<Cost> GetCost => cost;
	public CardType GetCardType => cardType;
	public override string ToString()
	{
		var text = $"{id}";
		if(cost.Count != 0)
		{
			var costLog = string.Empty;
			foreach (var c in cost)
			{
			    costLog += $"{c}, ";
			}
			text += $" (point:{point} cost:{costLog})";
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
		if(cardType != CardType.Cooking)
		{
			point = 0;
			cost.Clear();
		}
		id = name.Replace("CardFood", "").Replace("CardCook", "");
	}
}
