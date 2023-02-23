using System.Collections.Generic;
using UnityUtility;
using UnityEngine;
public class PlayerInfo
{
	public string id { get; private set; }
	public int coin { get; private set; }
	public Dictionary<CardData.CardType, List<CardInfo>> hand { get; private set; } = new Dictionary<CardData.CardType, List<CardInfo>>();
	public List<CardInfo> discard { get; private set; } = new List<CardInfo>();
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
		if(inCard == null)
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
	public void AddCoin()
	{
		foreach(var cards in hand)
		{
			foreach(var card in cards.Value)
			{
				coin += card.cardData.GetMoney;
			}
		}
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
	List<PlayerInfo> mPlayersInfo = new List<PlayerInfo>();
	public List<PlayerInfo> GetPlayerInfos => mPlayersInfo;
	public PlayersInfo(RandomObject inRand, Dictionary<string, Player> inPlayers)
	{
		foreach(var player in inPlayers)
		{
			mPlayersInfo.Add(new PlayerInfo(player.Key));
		}
		inRand.Shuffle(mPlayersInfo);
	}
	public PlayerInfo GetPlayerInfo(string inId)
	{
		foreach(var player in mPlayersInfo)
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
		foreach(var playerInfo in mPlayersInfo)
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
		foreach(var playerInfo in mPlayersInfo)
		{
			int point = playerInfo.GetPoint;
			if(point > max)
			{
				max = point;
			}
		}
		var playerList = new List<PlayerInfo>();
		foreach(var playerInfo in mPlayersInfo)
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
		return mPlayersInfo[inTurn % mPlayersInfo.Count];
	}
}
