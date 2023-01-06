using System.Collections.Generic;
using UnityUtility;
using UnityEngine;
public class PickInfo
{
	public int deck { get; set; }
	public int card { get; set; }
	public override string ToString()
	{
		return $"Pick deck:{deck}, card{card}";
	}
	public PickInfo(int inDeck, int inCard)
	{
		deck = inDeck;
		card = inCard;
	}
}
public class Card
{
	public int id { get; set; }
	public override string ToString()
	{
		return $"card:{id}";
	}
}
public class Deck
{
	List<Card> mCardList;
	public List<Card> GetCardList => mCardList;
	public Deck(RandomObject inRand)
	{
		Generate(inRand);
	}
	void Generate(RandomObject inRand, int inCount = 50)
	{
		mCardList = new List<Card>(inCount);
		for(int i = 0; i < inCount; ++i)
		{
			mCardList.Add(new Card { id = i });
		}
		inRand.Shuffle(mCardList);
	}
}
public class GameInfo
{
	List<Player> mTurnPlayer;
	List<Deck> mDeck;
	List<PickInfo> mPickInfo;
	public bool IsStart => mTurnPlayer != null;
	public override string ToString()
	{
		var str = $"pick:{mPickInfo.Count}\n";
		foreach(var player in mTurnPlayer)
		{
			var isTurn = IsTurn(player) ? ">" : " ";
			str += $"{isTurn}Player{player.OwnerClientId}\n";
		}
		return str;
	}
	public Player GetPickPlayer(int inDeck, int inCard)
	{
		int turn = GetPickTurn(inDeck, inCard);
		if(turn == -1)
		{
			return null;
		}
		return mTurnPlayer[turn % mTurnPlayer.Count];
	}
	public bool CanPick(int inDeck, int inCard)
	{
		return GetPickTurn(inDeck, inCard) == -1;
	}
	public List<Card> GetCardList(int inDeck)
	{
		return mDeck[inDeck].GetCardList;
	}
	public void GameStart(int inSeed, int inDeck, Player[] inPlayers, Vector3 inCenter, float inRadius)
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
		mDeck = new List<Deck>();
		for(int i = 0; i < inDeck; i++)
		{
			mDeck.Add(new Deck(rand));
		}
		mPickInfo = new List<PickInfo>();
		SetPlayerPos(inCenter, inRadius);
		Debug.Log($"GameStart\n {ToString()}");
	}
	public void Pick(int inDeck, int inCard)
	{
		if(mPickInfo == null)
		{
			return;
		}
		var pick = new PickInfo(inDeck, inCard);
		Debug.Log($"Pick {pick}\n {ToString()}");
		mPickInfo.Add(pick);
	}
	public bool IsTurn(Player inPlayer)
	{
		if(mTurnPlayer == null || mPickInfo == null)
		{
			return false;
		}
		return mTurnPlayer.IndexOf(inPlayer) == mPickInfo.Count % mTurnPlayer.Count;
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
}
