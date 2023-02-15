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
	Transform mDecks;
	GameObject mCardRoot;
	GameObject mCoinRoot;
	public bool IsTween => IsTweenCard(mCardRoot);
	Dictionary<string, List<GameObject>> mCoin = new Dictionary<string, List<GameObject>>();
	public void Apply(GameController inGameController)
	{
		Init(inGameController);
		mMenuRoot.Apply(inGameController);
		for(int i = 0; i < mDecks.childCount; ++i)
		{
			if(mDecks.GetChild(i).TryGetComponent<DeckModel>(out var deck))
			{
				deck.Apply(inGameController, mCardRoot.transform);
			}

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
	void Init(GameController inGameController)
	{
		if(mCardRoot != null)
		{
			return;
		}
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
}
