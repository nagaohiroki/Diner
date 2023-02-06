using System.Collections.Generic;
using UnityUtility;
using UnityEngine;
public class GameInfo
{
	List<string> mTurnPlayers;
	List<Deck> mDeck;
	List<PickInfo> mPickInfo;
	public bool IsStart => mTurnPlayers != null;
	public string GetCurrentTurnPlayer => GetTurnPlayer(mPickInfo.Count);
	public List<string> GetTurnPlayers => mTurnPlayers;
	public override string ToString()
	{
		return $"pick:{mPickInfo.Count}";
	}
	public void GameStart(BattleData inData, int inSeed, Dictionary<string, Player> inPlayers, PlayerChairs inPlayerChairs)
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
		mPickInfo = new List<PickInfo>();
		inPlayerChairs.Sitdown(mTurnPlayers, inPlayers);
		Debug.Log($"GameStart\n {ToString()}");
	}
	public void Pick(int inDeck, int inCard)
	{
		var pick = new PickInfo(inDeck, inCard);
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
	public List<int> Supply(int inDeck)
	{
		var deck = mDeck[inDeck];
		var cardList = GetCardList(inDeck);
		var list = new List<int>();
		for(int card = 0; card < cardList.Count; ++card)
		{
			int supply = SupplyIndex(inDeck, card);
			if(supply != -1)
			{
				list.Add(card);
			}
		}
		return list;
	}
	public bool CanPick(int inDeck, int inCard)
	{
		if(SupplyIndex(inDeck, inCard) == -1)
		{
			return false;
		}
		return HasCost(GetCurrentTurnPlayer, inDeck, inCard);
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
		foreach(var deck in mDeck)
		{
			if(deck.deckData.GetId == inId)
			{
				return deck;
			}
		}
		return null;
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
		var player = GetPickPlayer(inDeck, inCard);
		if(player == null)
		{
			return false;
		}
		var card = GetCard(inDeck, inCard);
		return GetTotalPaidCost(player, card.GetCardType) >= GetResourcePos(player, inDeck, inCard);
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
		return new PickInfo(maxDeck, maxCard);
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
	public int GetPoint(string inId)
	{
		int point = 0;
		for(int i = 0; i < mPickInfo.Count; ++i)
		{
			if(GetTurnPlayer(i) == inId)
			{
				point += GetPickCard(i).GetPoint;
			}
		}
		return point;
	}
	int GetResourcePos(string inPlayer, int inDeck, int inCard)
	{
		var targetCost = GetCard(inDeck, inCard).GetCardType;
		int num = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) != inPlayer)
			{
				continue;
			}
			var pick = mPickInfo[i];
			num += targetCost == GetCard(pick.deck, pick.card).GetCardType ? 1 : 0;
			if(pick.deck == inDeck && pick.card == inCard)
			{
				return num;
			}
		}
		return num;
	}
	string GetPickPlayer(int inDeck, int inCard)
	{
		return GetTurnPlayer(GetPickTurn(inDeck, inCard));
	}
	string GetTurnPlayer(int inTurn)
	{
		return inTurn < 0 ? null : mTurnPlayers[inTurn % mTurnPlayers.Count];
	}
	int GetPickTurn(int inDeck, int inCard)
	{
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			var pick = mPickInfo[i];
			if(pick.deck == inDeck && pick.card == inCard)
			{
				return i;
			}
		}
		return -1;
	}
	int SupplyIndex(int inDeck, int inCard)
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
	CardData GetPickCard(int inPickTurn)
	{
		var pick = mPickInfo[inPickTurn];
		return GetCard(pick.deck, pick.card);
	}
	bool HasCost(string inPlayer, int inDeck, int inCard)
	{
		var card = GetCard(inDeck, inCard);
		if(card.GetCost == null)
		{
			return true;
		}
		foreach(var newCost in card.GetCost)
		{
			if(GetHandResource(inPlayer, newCost.GetCostType) < newCost.GetNum)
			{
				return false;
			}
		}
		return true;
	}
	int GetHandResource(string inPlayer, CardData.CardType inCardType)
	{
		return GetTotalResource(inPlayer, inCardType) - GetTotalPaidCost(inPlayer, inCardType);
	}
	int GetTotalPaidCost(string inPlayer, CardData.CardType inCardType)
	{
		int num = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) == inPlayer)
			{
				num += GetPickCard(i).GetCostNum(inCardType);
			}
		}
		return num;
	}
	int GetTotalResource(string inPlayer, CardData.CardType inCardType)
	{
		int num = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) == inPlayer)
			{
				if(GetPickCard(i).GetCardType == inCardType)
				{
					++num;
				}
			}
		}
		return num;
	}
	Dictionary<CardData.CardType, float> CalcCardTypeScore()
	{
		var score = new Dictionary<CardData.CardType, float>();
		for(int deck = 0; deck < mDeck.Count; ++deck)
		{
			var supply = Supply(deck);
			for(int card = 0; card < supply.Count; ++card)
			{
				var remaindCostDict = new Dictionary<CardData.CardType, int>();
				var cardData = GetCard(deck, supply[card]);
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
