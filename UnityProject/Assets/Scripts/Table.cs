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
	public bool IsTween => IsTweenCard(mCardRoot);
	Dictionary<string, List<GameObject>> mCoin = new Dictionary<string, List<GameObject>>();
	GameController mGameContorller;
	public void Apply(GameController inGameController)
	{
		Init(inGameController);
		Coin(inGameController);
		mMenuRoot.Apply(inGameController);
		LayoutDeck(inGameController.gameInfo);
		Hand(inGameController);
		Discard(inGameController.gameInfo);
		Winner(inGameController);
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
	void LayoutDeck(GameInfo inGameinfo)
	{
		for(int i = 0; i < mDecks.childCount; ++i)
		{
			if(mDecks.GetChild(i).TryGetComponent<DeckModel>(out var deck))
			{
				deck.Layout(inGameinfo.GetDeck(deck.GetId), mCardRoot.transform);
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
		mCoinRoot = new GameObject("CoinRoot");
		mCoinRoot.transform.SetParent(mCardRoot.transform);
		foreach(var player in inGameController.gameInfo.GetPlayerInfos)
		{
			if(!mCoin.ContainsKey(player.id))
			{
				mCoin.Add(player.id, new List<GameObject>());
			}
		}
	}
	bool IsTweenCard(GameObject inGameObject)
	{
		if(LeanTween.isTweening(inGameObject))
		{
			return true;
		}
		for(int i = 0; i < inGameObject.transform.childCount; ++i)
		{
			var child = inGameObject.transform.GetChild(i);
			if(IsTweenCard(child.gameObject))
			{
				return true;
			}
		}
		return false;
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
	void Discard(GameInfo inInfo)
	{
		var players = inInfo.GetPlayerInfos;
		foreach(var player in players)
		{
			foreach(var card in player.discard)
			{
				var dis = FindCard(card);
				if(dis != null)
				{
					dis.gameObject.SetActive(false);
				}
			}
		}
	}
	CardModel FindCard(CardInfo inCard)
	{
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
	void Hand(GameController inGameController)
	{
		float tweenTime = 0.3f;
		var info = inGameController.gameInfo;
		var players = info.GetPlayerInfos;
		foreach(var playerInfo in players)
		{
			HandPlayer(info, playerInfo, inGameController.GetPlayer(playerInfo.id), tweenTime);
		}
	}
	void HandPlayer(GameInfo inInfo, PlayerInfo inPlayerInfo, Player inPlayer, float inTween)
	{
		var hands = inPlayerInfo.hand;
		float startX = 0.0f;
		foreach (var cards in hands)
		{
			startX -= cards.Value.Count * (IsPoint(cards.Key) ? mLayout.pointOffset.x : mLayout.handOffset.x) * 0.5f;
		}
		int counter = 0;
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
				CardOpen(inInfo, inPlayer, card, cardPos, inTween);
				++counter;
			}
		}
		foreach(var cards in hands)
		{
			if(!IsPoint(cards.Key))
			{
				continue;
			}
			int pointCounter = 0;
			foreach(var card in cards.Value)
			{
				var offset = mLayout.handOffset * counter;
				var pointOffset = mLayout.pointOffset * pointCounter;
				var cardPos = new Vector3(startX + offset.x + pointOffset.x, offset.y, 0.0f);
				CardOpen(inInfo, inPlayer, card, cardPos, inTween);
				++pointCounter;
			}
		}
	}
	void CardOpen(GameInfo inInfo, Player inPlayer, CardInfo inCard, Vector3 inPos, float inTween)
	{
		float rotY = inPlayer.rot;
		var rot = Quaternion.Euler(0.0f, rotY, 0.0f);
		var pos = inPlayer.transform.position + rot * (mLayout.handBaseOffset + Vector3.Scale(inPos, mLayout.handScale));
		var cardModel = GetCard(inCard, inInfo);
		LeanTween.move(cardModel.gameObject, pos, inTween);
		LeanTween.rotateY(cardModel.gameObject, rotY, inTween);
		var lt = LeanTween.scale(cardModel.gameObject, mLayout.handScale, inTween);
		cardModel.Open(lt);
	}
	bool IsPoint(CardData.CardType inType)
	{
		return inType == CardData.CardType.Bonus || inType == CardData.CardType.Cooking;
	}
	void Coin(GameController inGameController)
	{
		foreach(var playerInfo in inGameController.gameInfo.GetPlayerInfos)
		{
			var coins = mCoin[playerInfo.id];
			int coinCount = coins.Count;
			int diff = playerInfo.coin - coinCount;
			if(diff > 0)
			{
				for(int i = 0; i < diff; ++i)
				{
					var player = inGameController.GetPlayer(playerInfo.id);
					var coin = Instantiate(mCoinPrefab, mCoinRoot.transform);
					coins.Add(coin);
					coin.transform.position = player.transform.position + mLayout.coinOffset * coins.Count;
				}
			}
			if(diff < 0)
			{
				for(int i = 0; i < diff; ++i)
				{
					int last = coinCount - 1;
					Destroy(coins[last]);
					coins.RemoveAt(last);
				}
			}

		}
	}
#if UNITY_EDITOR
	void OnValidate()
	{
		if(mGameContorller != null)
		{
			Apply(mGameContorller);
		}
	}
#endif
}
