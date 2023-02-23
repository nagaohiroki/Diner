using System.Collections.Generic;
using UnityUtility;
public class GameInfo
{
	List<DeckInfo> mDeck;
	PickInfoList mPickInfo;
	PlayersInfo mPlayersInfo;
	public string GetCurrentTurnPlayer => mPlayersInfo.TurnPlayer(mPickInfo.Count).id;
	public PickInfoList GetPickInfoList => mPickInfo;
	public List<PlayerInfo> GetPlayerInfos => mPlayersInfo.GetPlayerInfos;
	public void GameStart(BattleData inData, int inSeed, Dictionary<string, Player> inPlayers, byte[] inPickData, RuleData inRule)
	{
		if(inPlayers == null)
		{
			return;
		}
		var rand = new RandomObject(inSeed);
		mDeck = new List<DeckInfo>();
		foreach(var deckData in inData.GetDeckList)
		{
			mDeck.Add(new DeckInfo(mDeck.Count, rand, inData, deckData, inRule));
		}
		mPlayersInfo = new PlayersInfo(rand, inPlayers);
		mPickInfo = PickInfoList.Load(inPickData);
		int count = 0;
		foreach(var pick in mPickInfo.picks)
		{
			mPlayersInfo.TurnPlayer(count).Pick(mDeck[pick.deck].Pick(pick.card));
			++count;
			mPlayersInfo.TurnPlayer(count).AddCoin();
		}
	}
	public void Pick(int inDeck, int inCard)
	{
		mPlayersInfo.TurnPlayer(mPickInfo.Count).Pick(mDeck[inDeck].Pick(inCard));
		mPickInfo.Add(new PickInfo { deck = inDeck, card = inCard });
		mPlayersInfo.TurnPlayer(mPickInfo.Count).AddCoin();
	}
	public bool CanPick(int inDeck, int inCard)
	{
		return mPlayersInfo.TurnPlayer(mPickInfo.Count).CanPick(mDeck[inDeck].GetCard(inCard));
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
	public DeckInfo GetDeck(string inId)
	{
		int index = GetDeckIndex(inId);
		return index != -1 ? mDeck[index] : null;
	}
	public List<PlayerInfo> GetWinners(int inWinPoint)
	{
		var win = mPlayersInfo.GetWinner(inWinPoint);
		if(win != null)
		{
			return new List<PlayerInfo> { win };
		}
		var player = mPlayersInfo.TurnPlayer(mPickInfo.Count);
		foreach(var deck in mDeck)
		{
			foreach(var supply in deck.supply)
			{
				if(player.CanPick(supply))
				{
					return null;
				}
			}
		}
		return mPlayersInfo.GetWinners();
	}
	public CardInfo AIPick()
	{
		foreach(var deck in mDeck)
		{
			foreach(var supply in deck.supply)
			{
				if(CanPick(supply.deckIndex, supply.cardIndex))
				{
					return supply;
				}
			}
		}
		return null;
	}
}
