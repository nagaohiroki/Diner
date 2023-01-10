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
	public override string ToString()
	{
		var str = $"pick:{mPickInfo.Count}\n";
		foreach(var player in mTurnPlayer)
		{
			var arrow = GetCurrentTurnPlayer == player ? ">" : " ";
			str += $"{arrow}Player{player.OwnerClientId}\n";
		}
		return str;
	}
	public void GameStart(GameData inGameData, int inSeed, int inDeck, Player[] inPlayers, Vector3 inCenter, float inRadius)
	{
		if(inPlayers == null)
		{
			return;
		}
		mTurnPlayer = new List<Player>();
		foreach(var player in inPlayers)
		{
			mTurnPlayer.Add(player);
		}
		var rand = new RandomObject(inSeed);
		rand.Shuffle(mTurnPlayer);
		mDeck = new List<Deck>
		{
			new Deck(rand, inGameData, new List<CostData.CostType>
			{
				CostData.CostType.Meat,
				CostData.CostType.Noodles,
				CostData.CostType.Sea,
				CostData.CostType.Vegetable,
				CostData.CostType.Milk,
				CostData.CostType.Sause,
				CostData.CostType.Soup,
				CostData.CostType.Potato,
			}),
			new Deck(rand, inGameData, new List<CostData.CostType>
			{
				CostData.CostType.Cooking
			}),
		};
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
		var player = GetPickPlayer(inDeck, inCard);
		if(player != null)
		{
			return false;
		}
		return HasCost(GetCurrentTurnPlayer, inDeck, inCard);
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
			if(GetPickPlayer(inDeck, i) == inPlayer)
			{
				if(inCard == i)
				{
					handIndex = handMax;
				}
				++handMax;
			}
		}
		return (handIndex, handMax);
	}
	public bool IsDiscard(int inDeck, int inCard)
	{
		var card = GetCard(inDeck, inCard);
		var player = GetPickPlayer(inDeck, inCard);
		return GetTotalCost(player, card.GetCostType) >= GetResourcePos(player, inDeck, inCard);
	}
	int GetResourcePos(Player inPlayer, int inDeck, int inCard)
	{
		var targetCost = GetCard(inDeck, inCard).GetCostType;
		int num = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) != inPlayer)
			{
				continue;
			}
			var pick = mPickInfo[i];
			num += targetCost == GetCard(pick.deck, pick.card).GetCostType ? 1 : 0;
			if(pick.deck == inDeck && pick.card == inCard)
			{
				return num;
			}
		}
		return num;
	}
	Player GetTurnPlayer(int inTurn)
	{
		if(inTurn < 0)
		{
			return null;
		}
		return mTurnPlayer[inTurn % mTurnPlayer.Count];
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
	int GetTotalCost(Player inPlayer, CostData.CostType inCostType)
	{
		int num = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) == inPlayer)
			{
				var pick = mPickInfo[i];
				var data = GetCard(pick.deck, pick.card);
				num += data.GetCostNum(inCostType);
			}
		}
		return num;
	}
	int GetTotalResource(Player inPlayer, CostData.CostType inCostType)
	{
		int num = 0;
		for(int i = 0; i < mPickInfo.Count; i++)
		{
			if(GetTurnPlayer(i) == inPlayer)
			{
				var pick = mPickInfo[i];
				var data = GetCard(pick.deck, pick.card);
				if(data.GetCostType == inCostType)
				{
					++num;
				}
			}
		}
		return num;
	}
}
