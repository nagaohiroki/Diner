using UnityEngine;
using System.Collections.Generic;
public class Table : MonoBehaviour
{
	[SerializeField]
	GameObject mTurnClock;
	[SerializeField]
	MenuRoot mMenuRoot;
	[SerializeField]
	AudioSource mWinnerSE;
	[SerializeField]
	GameObject mCoinPrefab;
	[SerializeField]
	List<DeckModel> mDeckModels;
	GameObject mCardRoot;
	GameObject mCoinRoot;
	public bool IsTween => IsTweenCard(mCardRoot);
	Dictionary<string, List<GameObject>> mCoin = new Dictionary<string, List<GameObject>>();
	public void Apply(GameController inGameController)
	{
		if(mCardRoot == null)
		{
			mCardRoot = new GameObject("CardRoot");
		}
		mMenuRoot.Apply(inGameController);
		foreach(var deck in mDeckModels)
		{
			deck.Apply(inGameController, mCardRoot.transform);
		}
		var winners = inGameController.gameInfo.GetWinners(inGameController.GetData.GetWinPoint);
		if(winners != null)
		{
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
			return;
		}
		LeanTween.delayedCall(1.0f, () =>
		{
			if(inGameController.GetCurrentTurnPlayer != null)
			{
				var trans = inGameController.GetCurrentTurnPlayer.transform;
				var rot = Quaternion.Euler(0.0f, inGameController.GetCurrentTurnPlayer.rot, 0.0f);
				LeanTween.move(mTurnClock, trans.position + rot * Vector3.forward * 2.0f, 0.2f);
			}
		});
		AddCoin(inGameController);
	}
	public void Clear()
	{
		Destroy(mCardRoot);
	}
	void AddCoin(GameController inGameController)
	{
		if(mCoinRoot == null)
		{
			mCoinRoot = new GameObject("CoinRoot");
			mCoinRoot.transform.SetParent(mCardRoot.transform);
		}
		var gameInfo = inGameController.gameInfo;
		var player = gameInfo.GetCurrentTurnPlayer;
		for(int i = 0; i < gameInfo.GetPickInfoList.Count; ++i)
		{
			if(gameInfo.GetTurnPlayer(i) != player)
			{
				continue;
			}
			var card = gameInfo.GetPickCard(i);
			int money = card.GetMoney;
			if(money == 0)
			{
				continue;
			}
			List<GameObject> coins = null;
			if(!mCoin.TryGetValue(player, out coins))
			{
				coins = new List<GameObject>();
				mCoin.Add(player, coins);
			}
			var pick = gameInfo.GetPickInfoList.Get(i);
			var cardModel = mDeckModels[pick.deck].GetCardModel(pick.card);
			var coin = Instantiate(mCoinPrefab, mCoinRoot.transform);
			coins.Add(coin);
			coin.SetActive(false);
			var offset = new Vector3(-0.12f, 0.0f, 0.0f) * coins.Count;
			coin.transform.position = cardModel.transform.position;
			coin.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -5.0f);
			var playerGo = inGameController.GetPlayer(player).gameObject;
			LeanTween.delayedCall(0.5f, () =>
			{
				coin.SetActive(true);
				LeanTween.move(coin, playerGo.transform.position + offset, 0.2f);
			});
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
}
