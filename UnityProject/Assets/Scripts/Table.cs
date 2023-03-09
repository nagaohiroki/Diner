using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LayoutParameter
{
	public Vector3 handScale = new Vector3(0.5f, 1.0f, 0.5f);
	public Vector3 handOffset = new Vector3(0.12f, 0.0001f, 0.0f);
	public Vector3 handBaseOffset = new Vector3(0.0f, 0.0f, -1.0f);
	public Vector3 pointOffset = new Vector3(0.12f, 0.0000f, -0.16f);
	public Vector3 coinOffset = new Vector3(0.2f, 0.0f, 0.0f);
}
public class Table : MonoBehaviour
{
	[SerializeField]
	MenuRoot mMenuRoot;
	[SerializeField]
	AudioSource mWinnerSE;
	[SerializeField]
	GameObject mCoinPrefab;
	[SerializeField]
	Transform mDecks;
	[SerializeField]
	LayoutParameter mLayout;
	GameObject mCardRoot;
	GameObject mCoinRoot;
	public bool IsTween => mSeqCounter != 0;
	int mSeqCounter = 0;
	Dictionary<string, List<GameObject>> mCoin;
	GameController mGameContorller;
	public void Apply(GameController inGameController, float inTweenTime)
	{
		mSeqCounter = 3;
		Init(inGameController);
		mMenuRoot.Apply(inGameController);
		var rootSeq = LeanTween.sequence();
		Discard(inGameController.gameInfo, inTweenTime, rootSeq);
		PayCoin(inGameController.gameInfo, inTweenTime, rootSeq);
		rootSeq.append(() =>
		{
			Hand(inGameController, inTweenTime, EndSeq);
			LayoutDeck(inGameController.gameInfo, inTweenTime, () =>
			{
				var seq = LeanTween.sequence();
				AddCoin(inGameController, inTweenTime, seq);
				seq.append(EndSeq);
			});
		});
		rootSeq.append(() => Winner(inGameController));
		rootSeq.append(EndSeq);
	}
	public void Clear()
	{
		if(mCardRoot != null)
		{
			Destroy(mCardRoot);
			mCardRoot = null;
		}
	}
	bool Winner(GameController inGameController)
	{
		var winners = inGameController.gameInfo.GetWinners(inGameController.GetData.GetWinPoint);
		if(winners == null)
		{
			return false;
		}
		var result = mMenuRoot.SwitchMenu<MenuResult>();
		var str = string.Empty;
		foreach(var winner in winners)
		{
			var player = inGameController.GetPlayer(winner.id);
			str += $"{player.name}\n";
		}
		str += $" is Win !!";
		result.SetText(str);
		mWinnerSE.Play();
		return true;
	}
	void EndSeq()
	{
		--mSeqCounter;
	}
	void LayoutDeck(GameInfo inGameinfo, float inTweenTime, System.Action inEnd)
	{
		int max = mDecks.childCount;
		int seqCounter = max;
		for(int i = 0; i < max; ++i)
		{
			if(mDecks.GetChild(i).TryGetComponent<DeckModel>(out var deck))
			{
				deck.Layout(inGameinfo.GetDeck(deck.GetId), mCardRoot.transform, inTweenTime, () =>
				{
					--seqCounter;
					if(seqCounter == 0)
					{
						inEnd();
					}
				});
			}
		}
	}
	void Init(GameController inGameController)
	{
		if(mCardRoot != null)
		{
			return;
		}
		mGameContorller = inGameController;
		mCardRoot = new GameObject("CardRoot");
		mCardRoot.transform.SetParent(transform);
		mCoinRoot = new GameObject("CoinRoot");
		mCoinRoot.transform.SetParent(mCardRoot.transform);
		mCoin = new Dictionary<string, List<GameObject>>();
		var playerInfos = inGameController.gameInfo.GetPlayerInfos;
		foreach(var playerInfo in playerInfos)
		{
			if(mCoin.ContainsKey(playerInfo.id))
			{
				continue;
			}
			var coins = new List<GameObject>();
			mCoin.Add(playerInfo.id, coins);
			var player = mGameContorller.GetPlayer(playerInfo.id);
			for(int i = 0; i < playerInfo.coin; ++i)
			{
				var coin = Instantiate(mCoinPrefab, mCoinRoot.transform);
				coins.Add(coin);
				coin.transform.position = CoinPos(player, coins.Count);
			}
		}
	}
	CardModel GetCard(CardInfo inCard, GameInfo inInfo)
	{
		for(int i = 0; i < mDecks.childCount; ++i)
		{
			var child = mDecks.GetChild(i);
			if(!child.TryGetComponent<DeckModel>(out var deckModel))
			{
				continue;
			}
			int deck = inInfo.GetDeckIndex(deckModel.GetId);
			if(deck == inCard.deckIndex)
			{
				return deckModel.CreateCardModel(inCard, mCardRoot.transform);
			}
		}
		return null;
	}
	void Discard(GameInfo inInfo, float inTweenTime, LTSeq inSeq)
	{
		var players = inInfo.GetPlayerInfos;
		foreach(var player in players)
		{
			foreach(var card in player.discard)
			{
				var dis = FindCard(card);
				if(dis != null && dis.gameObject.activeSelf)
				{
					dis.Discard(inSeq, FindCard(player.lastPickCard));
				}
			}
		}
	}
	CardModel FindCard(CardInfo inCard)
	{
		if(inCard == null)
		{
			return null;
		}
		var trans = mCardRoot.transform;
		for(int i = 0; i < trans.childCount; ++i)
		{
			var child = trans.GetChild(i);
			if(child.TryGetComponent<CardModel>(out var card))
			{
				if(card.IsSame(inCard))
				{
					return card;
				}
			}
		}
		return null;
	}
	void Hand(GameController inGameController, float inTweenTime, System.Action inEnd)
	{
		var info = inGameController.gameInfo;
		var players = info.GetPlayerInfos;
		int counter = 0;
		int max = players.Count;
		foreach(var playerInfo in players)
		{
			HandPlayer(info, playerInfo, inGameController.GetPlayer(playerInfo.id), inTweenTime, () =>
			{
				++counter;
				if(max == counter)
				{
					inEnd();
				}
			});
		}
	}
	void HandPlayer(GameInfo inInfo, PlayerInfo inPlayerInfo, Player inPlayer, float inTween, System.Action inEnd)
	{
		var hands = inPlayerInfo.hand;
		float startX = 0.0f;
		foreach(var cards in hands)
		{
			startX -= cards.Value.Count * (IsPoint(cards.Key) ? mLayout.pointOffset.x : mLayout.handOffset.x) * 0.5f;
		}
		int counter = 0;
		int pointCounter = 0;
		int seqCounter = 0;
		int total = inPlayerInfo.handTotal;
		if(total == 0)
		{
			inEnd();
		}
		void end()
		{
			++seqCounter;
			if(seqCounter == total)
			{
				inEnd();
			}
		};
		foreach(var cards in hands)
		{
			if(IsPoint(cards.Key))
			{
				continue;
			}
			foreach(var card in cards.Value)
			{
				var offset = mLayout.handOffset * counter;
				var cardPos = new Vector3(startX + offset.x, offset.y, 0.0f);
				CardOpen(inInfo, inPlayer, card, cardPos, inTween).append(end);
				++counter;
			}
		}
		foreach(var cards in hands)
		{
			if(!IsPoint(cards.Key))
			{
				continue;
			}
			foreach(var card in cards.Value)
			{
				var offset = mLayout.handOffset * counter;
				var pointOffset = mLayout.pointOffset * pointCounter;
				var cardPos = new Vector3(startX + offset.x + pointOffset.x, offset.y, 0.0f);
				CardOpen(inInfo, inPlayer, card, cardPos, inTween).append(end);
				++pointCounter;
			}
		}
	}
	LTSeq CardOpen(GameInfo inInfo, Player inPlayer, CardInfo inCard, Vector3 inPos, float inTweenTime)
	{
		float rotY = inPlayer.rot;
		var rot = Quaternion.Euler(0.0f, rotY, 0.0f);
		var pos = inPlayer.transform.position + rot * (mLayout.handBaseOffset + Vector3.Scale(inPos, mLayout.handScale));
		var cardModel = GetCard(inCard, inInfo);
		cardModel.supply = -1;
		var seq = LeanTween.sequence();
		if(pos != cardModel.transform.position)
		{
			seq.append(LeanTween.move(cardModel.gameObject, pos, inTweenTime).setEaseInOutExpo());
		}
		if(rotY != cardModel.transform.eulerAngles.y)
		{
			seq.append(LeanTween.rotateY(cardModel.gameObject, rotY, inTweenTime));
		}
		if(mLayout.handScale != cardModel.transform.localScale)
		{
			seq.append(LeanTween.scale(cardModel.gameObject, mLayout.handScale, inTweenTime));
		}
		cardModel.Open(seq);
		return seq;
	}
	bool IsPoint(CardData.CardType inType)
	{
		return inType == CardData.CardType.Bonus || inType == CardData.CardType.Cooking;
	}
	void PayCoin(GameInfo inInfo, float inTweenTime, LTSeq inSeq)
	{
		var players = inInfo.GetPlayerInfos;
		foreach(var playerInfo in players)
		{
			PayCoinPlayer(playerInfo, inTweenTime, inSeq);
		}
	}
	void PayCoinPlayer(PlayerInfo inPlayerInfo, float inTweenTime, LTSeq inSeq)
	{
		mCoin.TryGetValue(inPlayerInfo.id, out var coins);
		int paid = coins.Count - inPlayerInfo.coin;
		if(paid <= 0)
		{
			return;
		}
		var lastPickCard = FindCard(inPlayerInfo.lastPickCard);
		for(int i = 0; i < paid; i++)
		{
			int index = coins.Count - (i + 1);
			var coin = coins[index];
			inSeq.append(LeanTween.move(coin, lastPickCard.transform.position, inTweenTime));
			inSeq.append(LeanTween.scale(coin, Vector3.zero, inTweenTime));
			inSeq.append(() => Destroy(coin));
		}
		coins.RemoveRange(coins.Count - paid, paid);
	}
	void AddCoin(GameController inGameController, float inTweenTime, LTSeq inSeq)
	{
		var players = inGameController.gameInfo.GetPlayerInfos;
		foreach(var playerInfo in players)
		{
			var player = inGameController.GetPlayer(playerInfo.id);
			AddCoinPlayer(playerInfo, player, inTweenTime, inSeq);
		}
	}
	void AddCoinPlayer(PlayerInfo inPlayerInfo, Player inPlayer, float inTweenTime, LTSeq inSeq)
	{
		var hand = inPlayerInfo.hand;
		mCoin.TryGetValue(inPlayerInfo.id, out var coins);
		if(coins.Count >= inPlayerInfo.coin)
		{
			return;
		}
		foreach(var cards in hand)
		{
			foreach(var card in cards.Value)
			{
				int cardCoin = card.cardData.GetCoin;
				for(int i = 0; i < cardCoin; ++i)
				{
					var coin = Instantiate(mCoinPrefab, mCoinRoot.transform);
					coin.gameObject.SetActive(false);
					coins.Add(coin);
					var start = FindCard(card).transform.position;
					var end = CoinPos(inPlayer, coins.Count);
					coin.transform.position = start;
					var lt = LeanTween.move(coin.gameObject, end, inTweenTime);
					inSeq.append(() => coin.gameObject.SetActive(true));
					inSeq.append(lt);
				}
			}
		}
	}
	Vector3 CoinPos(Player inPlayer, int inCount)
	{
		var rot = Quaternion.Euler(0.0f, inPlayer.rot, 0.0f);
		return inPlayer.transform.position + rot * mLayout.coinOffset * inCount;
	}
#if UNITY_EDITOR
	void OnValidate()
	{
		if(mGameContorller != null)
		{
			Apply(mGameContorller, 0.0f);
		}
	}
#endif
}
