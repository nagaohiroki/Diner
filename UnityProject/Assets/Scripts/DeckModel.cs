using UnityEngine;
using System.Collections.Generic;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	GameObject mAnchor;
	[SerializeField]
	CardModel mCardModelPrefab;
	[SerializeField]
	GameObject mRoot;
	[SerializeField]
	GameObject mDeck;
	List<CardModel> mCardModels = new List<CardModel>();
	public void Apply(int inDeck, GameInfo inInfo)
	{
		var cardList = inInfo.GetCardList(inDeck);
		int anchorIndex = 0;
		for(int card = 0; card < cardList.Count; ++card)
		{
			var pickPlayer = inInfo.GetPickPlayer(inDeck, card);
			var cardModel = GetCreatedCard(card);
			if(pickPlayer != null && cardModel != null)
			{
				if(cardModel.player != pickPlayer)
				{
					cardModel.player = pickPlayer;
					cardModel.Move(pickPlayer.transform);
				}
				continue;
			}
			// 作成
			if(cardModel == null)
			{
				cardModel = CreateCardModel(inInfo, inDeck, card);
				mCardModels.Add(cardModel);
			}
			// 移動
			if(cardModel.ancherIndex != anchorIndex)
			{
				cardModel.ancherIndex = anchorIndex;
				var child = mAnchor.transform.GetChild(cardModel.ancherIndex);
				cardModel.Move(child);
			}
			++anchorIndex;
			if(anchorIndex >= mAnchor.transform.childCount)
			{
				return;
			}
		}
	}
	CardModel CreateCardModel(GameInfo inInfo, int inDeck, int inCard)
	{
		var pos = transform.position;
		var cardPos = new Vector3(pos.x, mRoot.transform.position.y, pos.z);
		var cardModel = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f));
		cardModel.Create(inInfo, inDeck, inCard);
		return cardModel;
	}
	void SetNum(float inBaseScale, int inNum)
	{
		if(inNum == 0)
		{
			gameObject.SetActive(false);
		}
		gameObject.SetActive(true);
		float deckScale = inBaseScale * inNum;
		var scale = mDeck.transform.localScale;
		scale.z = deckScale;
		mDeck.transform.localScale = scale;
		var pos = mRoot.transform.position;
		pos.y = deckScale * 0.5f;
		mRoot.transform.position = pos;
	}
	CardModel GetCreatedCard(int inCardIndex)
	{
		foreach(var cardModel in mCardModels)
		{
			if(cardModel.cardIndex == inCardIndex)
			{
				return cardModel;
			}
		}
		return null;
	}
}
