﻿using System.Collections.Generic;
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
	public (int index, int max) GetHand(int inDeck, int inCard, string inPlayer)
	{
		var cards = GetCardList(inDeck);
		int handIndex = 0;
		int handMax = 0;
		for(int i = 0; i < cards.Count; i++)
		{
			if(GetPickPlayer(inDeck, i) != inPlayer || IsDiscard(inDeck, i))
			{
				continue;
			}
			if(inCard == i)
			{
				handIndex = handMax;
			}
			++handMax;
		}
		return (handIndex, handMax);
	}
	public bool IsDiscard(int inDeck, int inCard)
	{
		var card = GetCard(inDeck, inCard);
		var player = GetPickPlayer(inDeck, inCard);
		return GetTotalCost(player, card.GetCardType) >= GetResourcePos(player, inDeck, inCard);
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
			int cost = newCost.GetNum + GetTotalCost(inPlayer, newCost.GetCostType);
			if(cost > resource)
			{
				return false;
			}
		}
		return true;
	}
	int GetTotalCost(string inPlayer, CardData.CardType inCardType)
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
