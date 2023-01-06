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
	List<ulong> mTurnPlayer;
	List<Deck> mDeck;
	List<PickInfo> mPickInfo;
	public bool IsStart => mTurnPlayer != null;
	public override string ToString()
	{
		var str = $"pickcount:{mPickInfo.Count}\n";
		foreach(var id in mTurnPlayer)
		{
			var isTurn = IsTurn(id) ? ">" : " ";
			str += $"{isTurn}Player{id}\n";
		}
		return str;
	}
	public void GameStart(int inSeed, Player[] inPlayers)
	{
		if(inPlayers == null || inPlayers.Length < 2)
		{
			Debug.Log("プレイヤーが足りない");
			return;
		}
		mTurnPlayer = new List<ulong>();
		foreach(var player in inPlayers)
		{
			mTurnPlayer.Add(player.OwnerClientId);
		}
		var rand = new RandomObject(inSeed);
		rand.Shuffle(mTurnPlayer);
		mDeck = new List<Deck>
		{
			new Deck(rand),
			new Deck(rand)
		};
		mPickInfo = new List<PickInfo>();
		Debug.Log($"GameStart\n {ToString()}");
	}
	public void Pick(int inCard, int inDeck)
	{
		if(mPickInfo == null)
		{
			return;
		}
		var pick = new PickInfo(inDeck, inCard);
		Debug.Log($"Pick {pick}\n {ToString()}");
		mPickInfo.Add(pick);
	}
	public bool IsTurn(ulong inOwnerId)
	{
		if(mTurnPlayer == null || mPickInfo == null)
		{
			return false;
		}
		return mTurnPlayer.IndexOf(inOwnerId) == mPickInfo.Count % mTurnPlayer.Count;
	}
}
