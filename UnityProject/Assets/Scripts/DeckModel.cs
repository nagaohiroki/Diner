using UnityEngine;
using System.Collections.Generic;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	Material mBackface;
	[SerializeField]
	string mId;
	[SerializeField]
	Vector3 mSupplyOffset;
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
	void ApplySupply(GameController inGameController)
	{
		var deck = inGameController.gameInfo.GetDeck(mId);
		int deckIndex = inGameController.gameInfo.GetDeckIndex(mId);
		int supply = 0;
		for(int card = 0; card < deck.GetCardList.Count; ++card)
		{
			if(inGameController.gameInfo.GetPickPlayer(deckIndex, card) != null)
			{
				continue;
			}
			var pos = transform.position + mSupplyOffset * (supply + 1);
			var cardModel = CreateCardModel(inGameController.gameInfo, deckIndex, card);
			cardModel.ToSupply(pos, supply);
			++supply;
			if(supply >= deck.deckData.GetSupply)
			{
				break;
			}
		}
	}
	void ApplyHand(GameController inGameController)
	{
		var deck = inGameController.gameInfo.GetDeck(mId);
		int deckIndex = inGameController.gameInfo.GetDeckIndex(mId);
		var hands = new Dictionary<string, List<CardModel>>();
		for(int card = 0; card < deck.GetCardList.Count; ++card)
		{
			var playerId = inGameController.gameInfo.GetPickPlayer(deckIndex, card);
			if(playerId == null)
			{
				continue;
			}
			List<CardModel> cards;
			if(!hands.TryGetValue(playerId, out cards))
			{
				cards = new List<CardModel>();
				hands.Add(playerId, cards);
			}
			var model = CreateCardModel(inGameController.gameInfo, deckIndex, card);
			if(inGameController.gameInfo.IsDiscard(deckIndex, card))
			{
				model.gameObject.SetActive(false);
				continue;
			}
			cards.Add(model);
		}
		float handScale = 0.5f;
		var cardOffset = new Vector3(0.6f, 0.0f, 1.0f);
		var deckOffest = 0.5f;
		foreach(var hand in hands)
		{
			var cardList = hand.Value;
			cardList.Sort((a, b) => SortHand(a, b, inGameController));
			for(int card = 0; card < cardList.Count; card++)
			{
				var cardModel = cardList[card];
				var pickPlayer = inGameController.GetPlayer(hand.Key);
				var cardPos = new Vector3(-cardOffset.x * cardList.Count * 0.5f + cardOffset.x * card, 0.0f, cardOffset.z * cardModel.deckIndex + deckOffest);
				LeanTween.move(cardModel.gameObject, pickPlayer.transform.position + Quaternion.Euler(0.0f, pickPlayer.rot, 0.0f) * cardPos, 0.3f);
				LeanTween.rotateY(cardModel.gameObject, pickPlayer.rot, 0.3f);
				LeanTween.scale(cardModel.gameObject, new Vector3(handScale, 1.0f, handScale), 0.3f);
			}
		}
	}
	public void Apply(GameController inGameController)
	{
		ApplySupply(inGameController);
		ApplyHand(inGameController);
	}
	public void Clear()
	{
		foreach(var card in mCardModels)
		{
			Destroy(card.gameObject);
		}
		mCardModels.Clear();
	}
	int SortHand(CardModel inA, CardModel inB, GameController inGameController)
	{
		var info = inGameController.gameInfo;
		var b = info.GetCard(inB.deckIndex, inB.cardIndex);
		var a = info.GetCard(inA.deckIndex, inA.cardIndex);
		return b.GetCardType - a.GetCardType;
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
