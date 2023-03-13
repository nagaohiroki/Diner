using System.Collections.Generic;
using UnityUtility;
public class GameInfo
{
	List<DeckInfo> mDeck;
	PlayersInfo mPlayersInfo;
	public PickInfoList pickInfo { get; private set; }
	public List<PlayerInfo> GetPlayerInfos => mPlayersInfo.playersInfo;
	public int turnOffset { get; set; }
	public PlayerInfo CurrentTurnPlayerInfo => mPlayersInfo.TurnPlayer(pickInfo.picks.Count - turnOffset);
	public override string ToString()
	{
		return $"turn {pickInfo}\n{mPlayersInfo}";
	}
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
		pickInfo = PickInfoList.Load(inPickData);
		int count = 0;
		foreach(var pick in pickInfo.picks)
		{
			mPlayersInfo.TurnPlayer(count).Pick(mDeck[pick.deck].Pick(pick.card));
			++count;
			mPlayersInfo.TurnPlayer(count).CleanUp();
		}
	}
	public void Pick(int inDeck, int inCard)
	{
		CurrentTurnPlayerInfo.Pick(mDeck[inDeck].Pick(inCard));
		pickInfo.Add(new PickInfo { deck = inDeck, card = inCard });
		var next = CurrentTurnPlayerInfo;
		next.CleanUp();
	}
	public bool CanPick(CardInfo inInfo)
	{
		return CurrentTurnPlayerInfo.CanPick(mDeck[inInfo.deckIndex].GetCard(inInfo.cardIndex));
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
		var player = CurrentTurnPlayerInfo;
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
		return mPlayersInfo.AIPick(mDeck, CurrentTurnPlayerInfo);
	}
}
