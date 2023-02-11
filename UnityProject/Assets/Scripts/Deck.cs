using System.Collections.Generic;
using UnityUtility;
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
