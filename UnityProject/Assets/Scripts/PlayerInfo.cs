using System.Collections.Generic;
using UnityUtility;
using UnityEngine;
public class CardScore
{
	public int num { get; set; }
	public float point { get; set; }
}
public class PlayerInfo
{
	public string id { get; private set; }
	public int coin { get; private set; }
	public Dictionary<CardData.CardType, List<CardInfo>> hand { get; private set; } = new Dictionary<CardData.CardType, List<CardInfo>>();
	public List<CardInfo> discard { get; private set; } = new List<CardInfo>();
	public override string ToString()
	{
		var str = $"{id}, point:{GetPoint}, coin:{coin}\n";
		foreach(var cards in hand)
		{
			str += $"{cards.Key} x{cards.Value.Count}\n";
		}
		return str;
	}
	public int GetPoint
	{
		get
		{
			int point = 0;
			foreach(var cards in hand)
			{
				foreach(var card in cards.Value)
				{
					point += card.cardData.GetPoint;
				}
			}
			return point;
		}
	}
	public PlayerInfo(string inId)
	{
		id = inId;
	}
	public bool CanPick(CardInfo inCard)
	{
		if(inCard == null || inCard.cardData.GetCardType == CardData.CardType.Bonus)
		{
			return false;
		}
		return GetPayCoin(inCard) <= coin;
	}
	public void Pick(CardInfo inCardData)
	{
		Pay(inCardData);
		AddHand(inCardData);
	}
	public void CleanUp()
	{
		AddCoin();
	}
	public float CalcScore(CardInfo inCard, Dictionary<CardData.CardType, CardScore> inTypeScore)
	{
		if(!CanPick(inCard))
		{
			return float.MinValue;
		}
		var data = inCard.cardData;
		float point = data.GetPoint + data.GetCoin * 0.5f;
		if(point > 0)
		{
			return point;
		}
		var type = data.GetCardType;
		if(!inTypeScore.TryGetValue(type, out var val))
		{
			return 0.0f;
		}
		int num = val.num - GetCardType(type);
		return num <= 0 ? 0.0f : (val.point / (float)num);
	}
	void AddHand(CardInfo inCard)
	{
		List<CardInfo> cards = null;
		var type = inCard.cardData.GetCardType;
		if(!hand.TryGetValue(type, out cards))
		{
			cards = new List<CardInfo>();
			hand.Add(type, cards);
		}
		cards.Add(inCard);
	}
	void AddCoin()
	{
		foreach(var cards in hand)
		{
			foreach(var card in cards.Value)
			{
				coin += card.cardData.GetCoin;
			}
		}
	}
	void RemoveHand(Cost inCost)
	{
		var type = inCost.GetCostType;
		if(hand.TryGetValue(type, out var cards))
		{
			int count = Mathf.Min(inCost.GetNum, cards.Count);
			for(int i = 0; i < count; ++i)
			{
				discard.Add(cards[i]);
			}
			cards.RemoveRange(0, count);
			if(cards.Count == 0)
			{
				hand.Remove(type);
			}
		}
	}
	void Pay(CardInfo inCard)
	{
		coin -= GetPayCoin(inCard);
		foreach(var cost in inCard.cardData.GetCost)
		{
			RemoveHand(cost);
		}
	}
	void AddBonus(CardInfo inCard)
	{
		foreach(var cost in inCard.cardData.GetBonusCosts)
		{
			if(GetBonusType(cost.GetBonusType) < cost.GetNum)
			{
				return;
			}
		}
		AddHand(inCard);
	}
	int GetPayCoin(CardInfo inCard)
	{
		int pay = 0;
		foreach(var cost in inCard.cardData.GetCost)
		{
			int diff = cost.GetNum - GetCardType(cost.GetCostType);
			if(diff > 0)
			{
				pay += diff;
			}
		}
		return pay;
	}
	int GetCardType(CardData.CardType inType)
	{
		return hand.TryGetValue(inType, out var cards) ? cards.Count : 0;
	}
	int GetBonusType(CardData.BonusType inType)
	{
		int count = 0;
		foreach(var cards in hand)
		{
			foreach(var card in cards.Value)
			{
				if(card.cardData.GetBonusType == inType)
				{
					++count;
				}
			}
		}
		return count;
	}
}
public class PlayersInfo
{
	public List<PlayerInfo> playersInfo { get; private set; } = new List<PlayerInfo>();
	public PlayersInfo(RandomObject inRand, Dictionary<string, Player> inPlayers)
	{
		foreach(var player in inPlayers)
		{
			playersInfo.Add(new PlayerInfo(player.Key));
		}
		inRand.Shuffle(playersInfo);
	}
	public override string ToString()
	{
		var str = string.Empty;
		foreach(var playerInfo in playersInfo)
		{
			str += $"{playerInfo.ToString()}\n";
		}
		return str;
	}
	public PlayerInfo GetPlayerInfo(string inId)
	{
		foreach(var player in playersInfo)
		{
			if(player.id == inId)
			{
				return player;
			}
		}
		return null;
	}
	public PlayerInfo GetWinner(int inPoint)
	{
		foreach(var playerInfo in playersInfo)
		{
			if(inPoint <= playerInfo.GetPoint)
			{
				return playerInfo;
			}
		}
		return null;
	}
	public List<PlayerInfo> GetWinners()
	{
		int max = int.MinValue;
		foreach(var playerInfo in playersInfo)
		{
			int point = playerInfo.GetPoint;
			if(point > max)
			{
				max = point;
			}
		}
		var playerList = new List<PlayerInfo>();
		foreach(var playerInfo in playersInfo)
		{
			if(max == playerInfo.GetPoint)
			{
				playerList.Add(playerInfo);
			}
		}
		return playerList;
	}
	public PlayerInfo TurnPlayer(int inTurn)
	{
		return playersInfo[inTurn % playersInfo.Count];
	}
	public CardInfo AIPick(List<DeckInfo> inDeck, int inTurn)
	{
		var calcScore = CalcTypeScore(inDeck);
		var player = TurnPlayer(inTurn);
		CardInfo pick = null;
		float max = float.MinValue;
		foreach(var deck in inDeck)
		{
			foreach(var supply in deck.supply)
			{
				float score = player.CalcScore(supply, calcScore);
				if(score > max)
				{
					max = score;
					pick = supply;
				}
			}
		}
		return pick;
	}
	Dictionary<CardData.CardType, CardScore> CalcTypeScore(List<DeckInfo> inDeck)
	{
		var score = new Dictionary<CardData.CardType, CardScore>();
		foreach(var deck in inDeck)
		{
			foreach(var card in deck.supply)
			{
				var data = card.cardData;
				if(data.GetPoint <= 0)
				{
					continue;
				}
				CardScore cardScore = null;
				foreach(var cost in data.GetCost)
				{
					var type = cost.GetCostType;
					if(!score.TryGetValue(type, out cardScore))
					{
						cardScore = new CardScore();
						score.Add(type, cardScore);
					}
					cardScore.num += cost.GetNum;
					cardScore.point += data.GetCostRatio(type);
				}
			}
		}
		return score;
	}
}
public class CardInfo
{
	public int deckIndex { get; set; }
	public int cardIndex { get; set; }
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
		if(!inRule.isCoin)
		{
			foreach(var card in mCardList)
			{
				card.cardData.notUseCoin = true;
			}
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
	}
}
