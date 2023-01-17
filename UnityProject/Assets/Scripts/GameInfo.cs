using System.Collections.Generic;
using UnityUtility;
using UnityEngine;
public class GameInfo
{
	List<Player> mTurnPlayer;
	List<Deck> mDeck;
	List<PickInfo> mPickInfo;
	public bool IsStart => mTurnPlayer != null;
	public Player GetCurrentTurnPlayer => GetTurnPlayer(mPickInfo.Count);
	int mWinPoint;
	public override string ToString()
	{
		var str = $"pick:{mPickInfo.Count}\n";
		foreach(var player in mTurnPlayer)
		{
			var arrow = GetCurrentTurnPlayer == player ? ">" : " ";
			str += $"{arrow}Player{player.name}:{GetPoint(player)}\n";
		}
		return str;
	}
	public void GameStart(BattleData inData, int inSeed, Player[] inPlayers, Vector3 inCenter, float inRadius)
	{
		if(inPlayers == null)
		{
			return;
		}
		mWinPoint = inData.GetWinPoint;
		mTurnPlayer = new List<Player>();
		foreach(var player in inPlayers)
		{
			mTurnPlayer.Add(player);
		}
		var rand = new RandomObject(inSeed);
		rand.Shuffle(mTurnPlayer);
		mDeck = new List<Deck>();
		foreach(var deckData in inData.GetDeckList)
		{
			mDeck.Add(new Deck(rand, inData, deckData));
		}
		mPickInfo = new List<PickInfo>();
		SetPlayerPos(inCenter, inRadius);
		Debug.Log($"GameStart\n {ToString()}");
	}
	public void Pick(int inDeck, int inCard)
	{
		var pick = new PickInfo(inDeck, inCard);
		Debug.Log($"{pick}\n {ToString()}");
		mPickInfo.Add(pick);
	}
	public Player GetPickPlayer(int inDeck, int inCard)
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
	public (int index, int max) GetHand(int inDeck, int inCard, Player inPlayer)
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
	public Player GetWinner()
	{
		foreach(var player in mTurnPlayer)
		{
			if(GetPoint(player) >= mWinPoint)
			{
				return player;
			}
		}
		return null;
	}
	int GetPoint(Player inPlayer)
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
	int GetResourcePos(Player inPlayer, int inDeck, int inCard)
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
	Player GetTurnPlayer(int inTurn)
	{
		return inTurn < 0 ? null : mTurnPlayer[inTurn % mTurnPlayer.Count];
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
	void SetPlayerPos(Vector3 inCenter, float inRadius)
	{
		int start = -1;
		int count = mTurnPlayer.Count;
		for(int i = 0; i < count; i++)
		{
			if(mTurnPlayer[i].IsOwner)
			{
				start = i;
			}
		}
		var baseRot = 1.0f / (float)count * 360.0f;
		for(int i = 0; i < count; i++)
		{
			int ownerBaseIndex = (start + i) % count;
			var rot = Quaternion.Euler(0.0f, baseRot * i, 0.0f);
			var pos = inCenter + rot * new Vector3(0.0f, 0.0f, -inRadius);
			mTurnPlayer[ownerBaseIndex].transform.position = pos;
		}
	}
	bool HasCost(Player inPlayer, int inDeck, int inCard)
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
	int GetTotalCost(Player inPlayer, CardData.CardType inCardType)
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
	int GetTotalResource(Player inPlayer, CardData.CardType inCardType)
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
