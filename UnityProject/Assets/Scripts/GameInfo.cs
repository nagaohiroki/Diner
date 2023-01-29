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
	int mWinPoint;
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
		mWinPoint = inData.GetWinPoint;
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
	public string GetPickPlayer(int inDeck, int inCard)
	{
		return GetTurnPlayer(GetPickTurn(inDeck, inCard));
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
			if(deck.deckData.GetSupply <= list.Count)
			{
				break;
			}
			if(GetPickTurn(inDeck, card) == -1)
			{
				list.Add(card);
			}
		}
		return list;
	}
	public bool CanPick(int inDeck, int inCard)
	{
		if(GetWinner() != null)
		{
			return false;
		}
		var player = GetPickPlayer(inDeck, inCard);
		if(player != null)
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
	public string GetWinner()
	{
		foreach(var player in mTurnPlayers)
		{
			if(GetPoint(player) >= mWinPoint)
			{
				return player;
			}
		}
		return null;
	}
	int GetPoint(string inId)
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
			int resource = GetTotalResource(inPlayer, newCost.GetCostType);
			int cost = newCost.GetNum + GetTotalPaidCost(inPlayer, newCost.GetCostType);
			if(cost > resource)
			{
				return false;
			}
		}
		return true;
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
}
