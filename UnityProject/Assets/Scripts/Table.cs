using UnityEngine;
using System.Collections.Generic;
public class Table : MonoBehaviour
{
	[SerializeField]
	GameObject mTurnClock;
	[SerializeField]
	MenuRoot mMenuRoot;
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
		foreach(var deck in mDeckModels)
		{
			deck.Apply(inGameController, mCardRoot.transform);
		}
		var winner = inGameController.gameInfo.GetWinner();
		if(winner != null)
		{
			var result = mMenuRoot.SwitchMenu<MenuResult>();
			var player = inGameController.GetPlayer(winner);
			result.SetText($"{player.name} is Win!!");
			return;
		}
		LeanTween.delayedCall(1.0f, () =>
		{
			var trans = inGameController.GetCurrentTurnPlayer.transform;
			var rot = Quaternion.Euler(0.0f, inGameController.GetCurrentTurnPlayer.rot, 0.0f);
			LeanTween.move(mTurnClock, trans.position + rot * Vector3.forward * 2.0f, 0.2f);
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
