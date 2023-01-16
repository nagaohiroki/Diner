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
[CreateAssetMenu]
public class CardData : ScriptableObject
{
	public enum CardType
	{
		// 料理
		Cooking,
		// 肉
		Meat,
		// 海鮮
		Sea,
		// 野菜
		Vegetable,
		// 乳製品
		Milk,
		// スパイス
		Spices,
		// 合計
		Num
	}
	[SerializeField]
	string id;
	[SerializeField]
	CardType cardType;
	[SerializeField]
	int point;
	[SerializeField]
	List<CostData> cost;
	public string GetId => id;
	public int GetPoint => point;
	public List<CostData> GetCost => cost;
	public CardType GetCardType => cardType;
	public override string ToString()
	{
		return id;
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
	}
}
