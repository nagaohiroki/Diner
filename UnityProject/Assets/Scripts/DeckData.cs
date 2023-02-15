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
public class Deck
{
	List<CardData> mCardList;
	public DeckData deckData { get; private set; }
	public List<CardData> GetCardList => mCardList;
	public Deck(RandomObject inRand, BattleData inData, DeckData inDeckData)
	{
		deckData = inDeckData;
		mCardList = inDeckData.GenerateCardList();
		inRand.Shuffle(mCardList);
	}
}
