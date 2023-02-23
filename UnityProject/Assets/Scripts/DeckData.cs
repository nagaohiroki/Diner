using System.Collections.Generic;
using UnityUtility;
using UnityEngine;
[System.Serializable]
public class CardCounter
{
	[SerializeField]
	CardData mCard;
	[SerializeField]
	int mNum;
	public CardData GetCard => mCard;
	public int GetNum => mNum;
	public override string ToString()
	{
		return $"{mCard} x{mNum}";
	}
	public void Set(CardData inCardData)
	{
		mCard = inCardData;
	}
}
[CreateAssetMenu]
public class DeckData : ScriptableObject
{
	[SerializeField]
	string id;
	[SerializeField]
	int supply;
	[SerializeField]
	List<CardCounter> mCard;
	public string GetId => id;
	public int GetSupply => supply;
	public List<CardData> GenerateCardList()
	{
		var list = new List<CardData>();
		foreach(var card in mCard)
		{
			for(int i = 0; i < card.GetNum; i++)
			{
				list.Add(card.GetCard);
			}
		}
		return list;
	}
	public override string ToString()
	{
		var log = $"{id}:\n";
		int total = 0;
		foreach(var card in mCard)
		{
			log += $"{card}\n";
			total += card.GetNum;
		}
		log += $"---\nTotal:{total}\n";
		return log;
	}
}
public class CardInfo
{
	public int deckIndex { get; set; }
	public int cardIndex { get; set; }
	public int supplyIndex { get; set; }
	public bool useCoin { get; set; }
	public CardData cardData { get; set; }
	public CardInfo(int inDeck, int inCard, CardData inCardData)
	{
		deckIndex = inDeck;
		cardIndex = inCard;
		cardData = inCardData;
	}
	public bool IsSame(CardInfo inCardInfo)
	{
		return inCardInfo.deckIndex == deckIndex && inCardInfo.cardIndex == cardIndex;
	}
	public override string ToString()
		{
			return $"{cardData}, deck:{deckIndex}, card:{cardIndex}";
		}
}
public class DeckInfo
{
	List<CardInfo> mCardList = new List<CardInfo>();
	public DeckData deckData { get; private set; }
	public List<CardInfo> GetCardList => mCardList;
	public List<CardInfo> supply { get; set; } = new List<CardInfo>();
	public DeckInfo(int inDeck, RandomObject inRand, BattleData inData, DeckData inDeckData, RuleData inRule)
	{
		deckData = inDeckData;
		var cards = inDeckData.GenerateCardList();
		inRand.Shuffle(cards);
		for(int card = 0; card < cards.Count; ++card)
		{
			mCardList.Add(new CardInfo(inDeck, card, cards[card]));
		}
		if(!inRule.isBonus)
		{
			mCardList.RemoveAll(card => card.cardData.IsBonus);
		}
		UpdateSupply();
	}
	public CardInfo GetCard(int inIndex)
	{
		foreach(var card in supply)
		{
			if(card.cardIndex == inIndex)
			{
				return card;
			}
		}
		return null;
	}
	public CardInfo Pick(int inIndex)
	{
		var cardData = GetCard(inIndex);
		supply.Remove(cardData);
		UpdateSupply();
		return cardData;
	}
	void UpdateSupply()
	{
		int diff = deckData.GetSupply - supply.Count;
		if(diff == 0)
		{
			return;
		}
		diff = Mathf.Min(diff, mCardList.Count);
		for(int i = 0; i < diff; ++i)
		{
			supply.Add(mCardList[i]);
		}
		mCardList.RemoveRange(0, diff);
		for(int i = 0; i < supply.Count; i++)
		{
			supply[i].supplyIndex = i;
		}
		foreach(var card in mCardList)
		{
			card.supplyIndex = -1;
		}
	}
}
