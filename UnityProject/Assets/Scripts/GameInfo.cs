using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
public class GameInfo
{
	static readonly List<CardData.CardType> resourceMoneyType = new List<CardData.CardType>
	{
		CardData.CardType.Meat,
		CardData.CardType.SeaFood,
		CardData.CardType.Vegetable,
		CardData.CardType.Milk,
		CardData.CardType.Spices,
		CardData.CardType.Grain,
		CardData.CardType.Rare,
	};
	List<string> mTurnPlayers;
	List<Deck> mDeck;
	PickInfoList mPickInfo;
	public bool IsStart => mTurnPlayers != null;
	public string GetCurrentTurnPlayer => GetTurnPlayer(mPickInfo.Count);
	public List<string> GetTurnPlayers => mTurnPlayers;
	public PickInfoList GetPickInfoList => mPickInfo;
	public override string ToString()
	{
		return $"pick:{mPickInfo.Count}";
	}
	public void GameStart(BattleData inData, int inSeed, Dictionary<string, Player> inPlayers, byte[] inPickData)
	{
		if(inPlayers == null)
		{
			return;
		}
		mTurnPlayers = new List<string>();
		foreach(var player in inPlayers)
		{
			mTurnPlayers.Add(player.Key);
		}
		var rand = new RandomObject(inSeed);
		rand.Shuffle(mTurnPlayers);
		mDeck = new List<Deck>();
		foreach(var deckData in inData.GetDeckList)
		{
			mDeck.Add(new Deck(rand, inData, deckData));
		}
		mPickInfo = PickInfoList.Load(inPickData);
	}
	public void Pick(int inDeck, int inCard)
	{
		var pick = new PickInfo { deck = inDeck, card = inCard };
		Debug.Log($"{pick}\n {ToString()}");
		mPickInfo.Add(pick);
	}
	public Dictionary<string, List<int>> Hand(int inDeck)
	{
		var cardList = GetCardList(inDeck);
		var hand = new Dictionary<string, List<int>>();
		for(int card = 0; card < cardList.Count; ++card)
		{
			foreach(var player in mTurnPlayers)
			{
				if(!hand.ContainsKey(player))
				{
					hand[player] = new List<int>();
				}
				if(GetPickPlayer(inDeck, card) == player)
				{
					if(!IsDiscard(inDeck, card))
					{
						hand[player].Add(card);
					}
				}
			}
		}
		return hand;
	}
	public bool CanPick(int inDeck, int inCard)
	{
		if(SupplyIndex(inDeck, inCard) == -1)
		{
			return false;
		}
		return OverCost(GetCurrentTurnPlayer, inDeck, inCard) <= GetMoney(GetCurrentTurnPlayer);
	}
	public int GetDeckIndex(string inId)
	{
		for(int i = 0; i < mDeck.Count; ++i)
		{
			if(mDeck[i].deckData.GetId == inId)
			{
				return i;
			}
		}
		return -1;
	}
	public Deck GetDeck(string inId)
	{
		int index = GetDeckIndex(inId);
		return index != -1 ? mDeck[index] : null;
	}
	public List<CardData> GetCardList(int inDeck)
	{
		return mDeck[inDeck].GetCardList;
	}
	public CardData GetCard(int inDeck, int inCard)
	{
		return GetCardList(inDeck)[inCard];
	}
	public bool IsDiscard(int inDeck, int inCard)
	{
		return GetPaidTurn(inDeck, inCard) != -1;
	}
	public PickInfo AIPick()
	{
		var cardTypeScore = CalcCardTypeScore();
		var pick = new List<(float score, int deck, int card)>();
		for(int deck = 0; deck < mDeck.Count; ++deck)
		{
			for(int card = 0; card < GetCardList(deck).Count; ++card)
			{
				var score = PickScore(deck, card, cardTypeScore);
				if(score >= 0)
				{
					pick.Add((score, deck, card));
				}
			}
		}
		if(pick.Count <= 0)
		{
			return null;
		}
		RandomObject.GetGlobal.Shuffle(pick);
		int maxDeck = int.MinValue;
		int maxCard = int.MinValue;
		float maxScore = float.MinValue;
		foreach(var p in pick)
		{
			if(p.score > maxScore)
			{
				maxScore = p.score;
				maxDeck = p.deck;
				maxCard = p.card;
			}
		}
		return new PickInfo { deck = maxDeck, card = maxCard };
	}
	public List<string> GetWinners(int inWinPoint)
	{
		foreach(var player in mTurnPlayers)
		{
			if(GetPoint(player) >= inWinPoint)
			{
				return new List<string> { player };
			}
		}
		for(int deck = 0; deck < mDeck.Count; ++deck)
		{
			var cardList = GetCardList(deck);
			for(int card = 0; card < cardList.Count; ++card)
			{
				if(CanPick(deck, card))
				{
					return null;
				}
			}
		}
		int maxPoint = int.MinValue;
		var players = new List<string>();
		foreach(var player in mTurnPlayers)
		{
			int point = GetPoint(player);
			if(point > maxPoint)
			{
				maxPoint = point;
			}
		}
		foreach(var player in mTurnPlayers)
		{
			if(GetPoint(player) == maxPoint)
			{
				players.Add(player);
			}
		}
		return players;
	}
	public int GetPoint(string inPlayer)
	{
		int point = 0;
		for(int i = 0; i < mPickInfo.Count; ++i)
		{
			if(GetTurnPlayer(i) == inPlayer)
			{
				point += GetPickCard(i).GetPoint;
			}
		}
		return point;
	}
	public int GetMoney(string inPlayer)
	{
		int totalMoney = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) == inPlayer)
			{
				var card = GetPickCard(i);
				int money = card.GetMoney;
				if(money == 0)
				{
					continue;
				}
				int turn = GetTurnCount(inPlayer, mPickInfo.Count) - GetTurnCount(inPlayer, i);
				totalMoney += money * turn;
			}
		}
		int paid = 0;
		foreach(var res in resourceMoneyType)
		{
			paid += GetTypeMoneyCost(inPlayer, res);
		}
		return totalMoney - paid;
	}
	public int SupplyIndex(int inDeck, int inCard)
	{
		if(GetPickTurn(inDeck, inCard) != -1)
		{
			return -1;
		}
		var deck = mDeck[inDeck];
		int supply = 0;
		for(int card = 0; card < deck.GetCardList.Count; ++card)
		{
			if(GetPickTurn(inDeck, card) != -1)
			{
				continue;
			}
			if(card == inCard)
			{
				return supply;
			}
			++supply;
			if(supply >= deck.deckData.GetSupply)
			{
				break;
			}
		}
		return -1;
	}
	int GetTypeMoneyCost(string inPlayer, CardData.CardType inType)
	{
		int money = 0;
		int resource = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) != inPlayer)
			{
				continue;
			}
			var card = GetPickCard(i);
			if(card.GetCardType == inType)
			{
				++resource;
			}
			var costs = card.GetCost;
			if(costs == null)
			{
				continue;
			}
			foreach(var cost in costs)
			{
				if(cost.GetCostType != inType)
				{
					continue;
				}
				resource -= cost.GetNum;
				if(resource < 0)
				{
					money -= resource;
				}
			}
		}
		return money;
	}
	int GetPaidTurn(int inDeck, int inCard)
	{
		var player = GetPickPlayer(inDeck, inCard);
		if(player == null)
		{
			return -1;
		}
		var type = GetCard(inDeck, inCard).GetCardType;
		int resource = 0;
		int resourcePos = -1;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) != player)
			{
				continue;
			}
			var card = GetPickCard(i);
			if(card.GetCardType == type)
			{
				++resource;
				var pick = mPickInfo.Get(i);
				if(pick.deck == inDeck && pick.card == inCard)
				{
					resourcePos = resource;
				}
			}
			if(card.GetCost == null)
			{
				continue;
			}
			foreach(var cost in card.GetCost)
			{
				if(cost.GetCostType != type)
				{
					continue;
				}
				resource -= cost.GetNum;
				if(resource < resourcePos)
				{
					return i;
				}
				if(resource <= 0)
				{
					resourcePos = -1;
					resource = 0;
				}
			}
		}
		return -1;
	}
	string GetPickPlayer(int inDeck, int inCard)
	{
		return GetTurnPlayer(GetPickTurn(inDeck, inCard));
	}
	public string GetTurnPlayer(int inTurn)
	{
		return inTurn < 0 ? null : mTurnPlayers[inTurn % mTurnPlayers.Count];
	}
	int GetPickTurn(int inDeck, int inCard)
	{
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			var pick = mPickInfo.Get(i);
			if(pick.deck == inDeck && pick.card == inCard)
			{
				return i;
			}
		}
		return -1;
	}
	public CardData GetPickCard(int inPickTurn)
	{
		var pick = mPickInfo.Get(inPickTurn);
		return GetCard(pick.deck, pick.card);
	}
	int OverCost(string inPlayer, int inDeck, int inCard)
	{
		int overCost = 0;
		var card = GetCard(inDeck, inCard);
		if(card.GetCost == null)
		{
			return 0;
		}
		foreach(var newCost in card.GetCost)
		{
			int over = newCost.GetNum - GetHandResource(inPlayer, newCost.GetCostType);
			if(over > 0)
			{
				overCost += over;
			}
		}
		return overCost;
	}
	int GetHandResource(string inPlayer, CardData.CardType inCardType)
	{
		int count = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			var pick = mPickInfo.Get(i);
			if(GetTurnPlayer(i) == inPlayer && GetPaidTurn(pick.deck, pick.card) == -1 && GetPickCard(i).GetCardType == inCardType)
			{
				++count;
			}
		}
		return count;
	}
	int GetTurnCount(string inPlayer, int inTurn)
	{
		return Mathf.Max(0, (inTurn - mTurnPlayers.IndexOf(inPlayer)) / mTurnPlayers.Count);
	}
	Dictionary<CardData.CardType, float> CalcCardTypeScore()
	{
		var score = new Dictionary<CardData.CardType, float>();
		for(int deck = 0; deck < mDeck.Count; ++deck)
		{
			var cardList = GetCardList(deck);
			for(int card = 0; card < cardList.Count; ++card)
			{
				if(SupplyIndex(deck, card) == -1)
				{
					continue;
				}
				var remaindCostDict = new Dictionary<CardData.CardType, int>();
				var cardData = GetCard(deck, card);
				int totalCost = 0;
				foreach(var cost in cardData.GetCost)
				{
					int remaindCost = cost.GetNum - GetHandResource(GetCurrentTurnPlayer, cost.GetCostType);
					if(remaindCost > 0)
					{
						totalCost += remaindCost;
						if(!remaindCostDict.ContainsKey(cost.GetCostType))
						{
							remaindCostDict.Add(cost.GetCostType, 0);
						}
						remaindCostDict[cost.GetCostType] += remaindCost;
					}
				}
				foreach(var remaind in remaindCostDict)
				{
					if(!score.ContainsKey(remaind.Key))
					{
						score.Add(remaind.Key, 0);
					}
					score[remaind.Key] += (float)cardData.GetPoint / (float)totalCost;
				}
			}
		}
		return score;
	}
	float PickScore(int inDeck, int inCard, Dictionary<CardData.CardType, float> inScore)
	{
		if(!CanPick(inDeck, inCard))
		{
			return -1;
		}
		var cardData = GetCard(inDeck, inCard);
		int point = cardData.GetPoint;
		if(point > 0)
		{
			return cardData.GetPoint * 100.0f;
		}
		if(inScore.TryGetValue(cardData.GetCardType, out var val))
		{
			return val;
		}
		return 0;
	}
}
