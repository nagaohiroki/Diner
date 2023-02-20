using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LayoutParameter
{
	public float handScale = 0.5f;
	public Vector3 handOffset = new Vector3(0.6f, 0.001f, -0.1f);
	public Vector3 handBaseOffset = new Vector3(0.0f, 0.0f, -0.5f);
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
		mMenuRoot.Apply(inGameController);
		LayoutDeck(inGameController);
		LayoutHand(inGameController);
		if(Winner(inGameController))
		{
			return;
		}
		ApplyCoin(inGameController);
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
			var player = inGameController.GetPlayer(winner);
			str += $"{player.name}\n";
		}
		str += $" is Win !!";
		result.SetText(str);
		mWinnerSE.Play();
		return true;
	}
	void LayoutDeck(GameController inGameController)
	{
		for(int i = 0; i < mDecks.childCount; ++i)
		{
			if(mDecks.GetChild(i).TryGetComponent<DeckModel>(out var deck))
			{
				deck.Apply(inGameController, mCardRoot.transform);
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
		foreach(var player in inGameController.gameInfo.GetTurnPlayers)
		{
			if(!mCoin.ContainsKey(player))
			{
				mCoin.Add(player, new List<GameObject>());
			}
		}
	}
	void ApplyCoin(GameController inGameController)
	{
		var gameInfo = inGameController.gameInfo;
		int count = gameInfo.GetPickInfoList.Count;
		var offset = new Vector3(-0.15f, 0.0f, 0.0f);
		var rot = Quaternion.Euler(0.0f, 0.0f, -5.0f);
		for(int turn = 0; turn < count; ++turn)
		{
			var player = gameInfo.GetTurnPlayer(turn);
			int money = gameInfo.GetMoney(player);
			var coins = mCoin[player];
			int coinCount = coins.Count;
			int diff = money - coinCount;
			if(diff > 0)
			{
				for(int i = 0; i < diff; ++i)
				{
					var playerGo = inGameController.GetPlayer(player).gameObject;
					var coin = Instantiate(mCoinPrefab, mCoinRoot.transform);
					coins.Add(coin);
					coin.transform.position = playerGo.transform.position + offset * coins.Count;
					coin.transform.rotation = rot;
				}
				continue;
			}
			if(diff < 0)
			{
				for(int i = coinCount - 1; i >= money; --i)
				{
					Destroy(coins[i]);
					coins.RemoveAt(i);
				}
				continue;
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
	CardModel GetCard(GameInfo inInfo, int inDeck, int inCard)
	{
		for(int i = 0; i < mDecks.childCount; ++i)
		{
			var child = mDecks.GetChild(i);
			if(!child.TryGetComponent<DeckModel>(out var deckModel))
			{
				continue;
			}
			int deck = inInfo.GetDeckIndex(deckModel.GetId);
			if(deck == inDeck)
			{
				return deckModel.CreateCardModel(inInfo, inDeck, inCard);
			}
		}
		return null;
	}
	void LayoutHand(GameController inGameController)
	{
		if(mGameContorller == null)
		{
			return;
		}
		var info = inGameController.gameInfo;
		if(info == null)
		{
			return;
		}
		var hands = info.GetHand();
		foreach(var hand in hands)
		{
			float typeOffsetX = 0.0f;
			var typeList = hand.Value;
			var startX = (typeList.Count - 1) * mLayout.handOffset.x * -0.5f;
			foreach(var typeCardList in typeList)
			{
				var cardList = typeCardList.Value;
				for(int card = 0; card < cardList.Count; ++card)
				{
					float posY = card * mLayout.handOffset.y;
					var cardData = cardList[card];
					var cardModel = GetCard(info, cardData.deck, cardData.card);
					var pickPlayer = inGameController.GetPlayer(hand.Key);
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
#if UNITY_EDITOR
	void OnValidate()
	{
		LayoutHand(mGameContorller);
	}
#endif
}
