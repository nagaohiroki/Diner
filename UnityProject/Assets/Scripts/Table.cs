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
	List<DeckModel> mDeckModels;
	GameObject mCardRoot;
	public bool IsTween => IsTweenCard(mCardRoot);
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
	}
	public void Clear()
	{
		Destroy(mCardRoot);
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
