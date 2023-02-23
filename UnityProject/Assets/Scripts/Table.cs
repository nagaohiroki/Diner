using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LayoutParameter
{
	public float handScale = 0.5f;
	public Vector3 handOffset = new Vector3(0.6f, 0.001f, -0.1f);
	public Vector3 handBaseOffset = new Vector3(0.0f, 0.0f, -0.5f);
	public Vector3 coinOffset = new Vector3(1.0f, 0.0f, 0.0f);
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
				deck.Layout(inGameinfo, mCardRoot.transform);
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
		var info = inGameController.gameInfo;
		var players = info.GetPlayerInfos;
		foreach(var playerInfo in players)
		{
			var pickPlayer = inGameController.GetPlayer(playerInfo.id);
			var hands = playerInfo.hand;
			float typeOffsetX = 0.0f;
			var startX = (hands.Count - 1) * mLayout.handOffset.x * -0.5f;
			foreach(var typeCardList in hands)
			{
				var cardList = typeCardList.Value;
				for(int card = 0; card < cardList.Count; ++card)
				{
					float posY = card * mLayout.handOffset.y;
					var cardModel = GetCard(cardList[card], info);
					var cardPos = mLayout.handBaseOffset + new Vector3(startX + typeOffsetX, posY, card * mLayout.handOffset.z);
					LeanTween.move(cardModel.gameObject, pickPlayer.transform.position + Quaternion.Euler(0.0f, pickPlayer.rot, 0.0f) * cardPos, 0.3f);
					LeanTween.rotateY(cardModel.gameObject, pickPlayer.rot, 0.3f);
					var lt = LeanTween.scale(cardModel.gameObject, new Vector3(mLayout.handScale, 1.0f, mLayout.handScale), 0.3f);
					cardModel.Open(lt);
				}
				typeOffsetX += mLayout.handOffset.x;
			}
		}
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
