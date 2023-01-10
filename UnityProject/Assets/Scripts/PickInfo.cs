using System.Collections.Generic;
using UnityUtility;
public class PickInfo
{
	public int deck { get; set; }
	public int card { get; set; }
	public override string ToString()
	{
		return $"Pick deck:{deck}, card{card}";
	}
	public PickInfo(int inDeck, int inCard)
	{
		deck = inDeck;
		card = inCard;
	}
}
public class Deck
{
	List<CardData> mCardList;
	public List<CardData> GetCardList => mCardList;
	public Deck(RandomObject inRand, GameData inData, DeckData inDeckData)
	{
		mCardList = new List<CardData>();
		foreach(var card in inData.GetCardList)
		{
			if(inDeckData.costType.Contains(card.GetCostType))
			{
				mCardList.Add(card);
			}
		}
		inRand.Shuffle(mCardList);
	}
}
