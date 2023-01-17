using UnityEngine;
using System.Collections.Generic;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	string mId;
	[SerializeField]
	GameObject mAnchor;
	[SerializeField]
	CardModel mCardModelPrefab;
	[SerializeField]
	GameObject mRoot;
	[SerializeField]
	GameObject mDeck;
	List<CardModel> mCardModels = new List<CardModel>();
	public void Apply(GameInfo inInfo)
	{
		var deck = inInfo.GetDeck(mId);
		var cardList = deck.GetCardList;
		int deckIndex = inInfo.GetDeckIndex(mId);
		int supply = 0;
		for(int card = 0; card < cardList.Count; ++card)
		{
			var cardModel = CreateCardModel(inInfo, deckIndex, card);
			if(Hand(inInfo, deckIndex, card, cardModel))
			{
				continue;
			}
			if(cardModel.supplyIndex != supply)
			{
				cardModel.supplyIndex = supply;
				var child = mAnchor.transform.GetChild(cardModel.supplyIndex);
				LeanTween.move(cardModel.gameObject, child.position, 0.3f)
					.setOnComplete(() => LeanTween.rotateZ(cardModel.gameObject, 0.0f, 0.5f));
			}
			++supply;
			if(supply >= deck.deckData.GetSupply)
			{
				return;
			}
		}
	}
	public void Clear()
	{
		foreach(var card in mCardModels)
		{
			Destroy(card.gameObject);
		}
		mCardModels.Clear();
	}
	bool Hand(GameInfo inInfo, int inDeck, int inCard, CardModel inCardModel)
	{
		var pickPlayer = inInfo.GetPickPlayer(inDeck, inCard);
		if(pickPlayer == null)
		{
			return false;
		}
		if(inInfo.IsDiscard(inDeck, inCard))
		{
			inCardModel.gameObject.SetActive(false);
			return true;
		}
		var pos = pickPlayer.transform.position;
		float offset = 0.75f;
		(int handIndex, int handMax) = inInfo.GetHand(inDeck, inCard, pickPlayer);
		pos.x += -offset * handMax * 0.5f + offset * handIndex;
		LeanTween.move(inCardModel.gameObject, pos, 0.3f);
		return true;
	}
	CardModel CreateCardModel(GameInfo inInfo, int inDeck, int inCard)
	{
		foreach(var card in mCardModels)
		{
			if(card.cardIndex == inCard)
			{
				return card;
			}
		}
		var pos = transform.position;
		var cardPos = new Vector3(pos.x, mRoot.transform.position.y, pos.z);
		var cardModel = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f));
		cardModel.Create(inInfo, inDeck, inCard);
		mCardModels.Add(cardModel);
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
}
