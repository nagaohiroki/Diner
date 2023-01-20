using UnityEngine;
using System.Collections.Generic;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	Material mBackface;
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
	[SerializeField]
	MeshRenderer mBackfaceMesh;
	Material mCache;
	List<CardModel> mCardModels = new List<CardModel>();
	public void Apply(GameInfo inInfo, GameController inGameController)
	{
		var deck = inInfo.GetDeck(mId);
		var cardList = deck.GetCardList;
		int deckIndex = inInfo.GetDeckIndex(mId);
		int supply = 0;
		for(int card = 0; card < cardList.Count; ++card)
		{
			var cardModel = CreateCardModel(inInfo, deckIndex, card);
			if(Hand(inInfo, deckIndex, card, cardModel, inGameController))
			{
				continue;
			}
			if(cardModel.supplyIndex != supply)
			{
				var child = mAnchor.transform.GetChild(supply);
				var go = cardModel.gameObject;
				var lt = LeanTween.move(go, child.position, 0.3f);
				if(cardModel.supplyIndex == -1)
				{
					lt.setOnComplete(() => LeanTween.moveY(go, 0.5f, 0.1f)
					.setOnComplete(() => LeanTween.rotateZ(go, 0.0f, 0.3f)
					.setOnComplete(() => LeanTween.moveY(go, 0.0f, 0.1f))));
				}
				cardModel.supplyIndex = supply;
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
	bool Hand(GameInfo inInfo, int inDeck, int inCard, CardModel inCardModel, GameController inGameController)
	{
		var pickId = inInfo.GetPickPlayer(inDeck, inCard);
		if(pickId == null)
		{
			return false;
		}
		var go = inCardModel.gameObject;
		if(inInfo.IsDiscard(inDeck, inCard))
		{
			go.SetActive(false);
			return true;
		}
		float handScale = 0.5f;
		var cardOffset = new Vector3(0.6f, 0.0f, 1.0f);
		var deckOffest = 0.5f;
		var pickPlayer = inGameController.GetPlayer(pickId);
		(int handIndex, int handMax) = inInfo.GetHand(inDeck, inCard, pickId);
		var cardPos = new Vector3(-cardOffset.x * handMax * 0.5f + cardOffset.x * handIndex, 0.0f, cardOffset.z * inDeck + deckOffest);
		LeanTween.move(go, pickPlayer.transform.position + Quaternion.Euler(0.0f, pickPlayer.rot, 0.0f) * cardPos, 0.3f);
		LeanTween.rotateY(go, pickPlayer.rot, 0.3f);
		LeanTween.scale(go, new Vector3(handScale, 1.0f, handScale), 0.3f);
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
		cardModel.Create(inInfo, inDeck, inCard, mBackface);
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
	void Start()
	{
		mBackfaceMesh.material = mBackface;
		mCache = mBackfaceMesh.material;
	}
	void OnDestroy()
	{
		if(mCache != null)
		{
			Destroy(mCache);
		}
	}
}
