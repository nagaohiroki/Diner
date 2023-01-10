using System.Collections.Generic;
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
	int point;
	[SerializeField]
	string id;
	[SerializeField]
	CostData.CostType costType;
	[SerializeField]
	List<CostData> cost;
	public string GetId => id;
	public int GetPoint => point;
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
			point = 1,
			cost = new List<CostData> { CostData.Dummy() },
		};
	}
}
[System.Serializable]
public class DeckData
{
	public List<CostData.CostType> costType;
}
public class GameData : MonoBehaviour
{
	[SerializeField]
	int winPoint;
	[SerializeField]
	List<DeckData> deckList;
	[SerializeField]
	List<CardData> cardList;
	public List<CardData> GetCardList => cardList;
	public List<DeckData> GetDeckList => deckList;
	public int GetWinPoint => winPoint;
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
		deckList.Clear();
		deckList.Add(new DeckData
		{
			costType = new List<CostData.CostType>
			{
				CostData.CostType.Meat,
				CostData.CostType.Noodles,
				CostData.CostType.Sea,
				CostData.CostType.Vegetable,
				CostData.CostType.Milk,
				CostData.CostType.Sause,
				CostData.CostType.Soup,
				CostData.CostType.Potato,
			}
		});
		deckList.Add(new DeckData
		{
			costType = new List<CostData.CostType>
			{
				CostData.CostType.Cooking,
			}
		});
	}
}
