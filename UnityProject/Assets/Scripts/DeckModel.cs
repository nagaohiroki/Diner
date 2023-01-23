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
	public void Apply(GameController inGameController)
	{
		var deck = inGameController.gameInfo.GetDeck(mId);
		int deckIndex = inGameController.gameInfo.GetDeckIndex(mId);
		int supply = 0;
		int count = deck.GetCardList.Count;
		for(int card = 0; card < count; ++card)
		{
			var cardModel = CreateCardModel(inGameController.gameInfo, deckIndex, card);
			if(cardModel.Hand(deckIndex, card, inGameController))
			{
				continue;
			}
			cardModel.ToSupply(mAnchor.transform.GetChild(supply).position, supply);
			++supply;
			if(supply >= deck.deckData.GetSupply)
			{
				break;
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
