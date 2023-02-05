using UnityEngine;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	Material mBackface;
	[SerializeField]
	string mId;
	[SerializeField]
	Vector3 mSupplyOffset;
	[SerializeField]
	float mSEPitch = 1.0f;
	[SerializeField]
	CardModel mCardModelPrefab;
	[SerializeField]
	GameObject mRoot;
	[SerializeField]
	GameObject mDeck;
	[SerializeField]
	MeshRenderer mBackfaceMesh;
	Material mCache;
	Transform mCardRoot;
	public void Apply(GameController inGameController, Transform inCardRoot)
	{
		if(mCardRoot == null)
		{
			var root = new GameObject(mId);
			root.transform.SetParent(inCardRoot);
			mCardRoot = root.transform;
		}
		ApplySupply(inGameController);
		ApplyHand(inGameController);
		ApplyDiscard(inGameController);
	}
	void ApplySupply(GameController inGameController)
	{
		int deckIndex = inGameController.gameInfo.GetDeckIndex(mId);
		var supplyList = inGameController.gameInfo.Supply(deckIndex);
		for(int supply = 0; supply < supplyList.Count; supply++)
		{
			var pos = transform.position + mSupplyOffset * (supply + 1);
			var cardModel = CreateCardModel(inGameController.gameInfo, deckIndex, supplyList[supply]);
			cardModel.ToSupply(pos, supply, mSEPitch);
		}
	}
	void ApplyHand(GameController inGameController)
	{
		int deckIndex = inGameController.gameInfo.GetDeckIndex(mId);
		var hands = inGameController.gameInfo.Hand(deckIndex);
		float handScale = 0.5f;
		var cardOffset = new Vector3(0.6f, 0.0f, 1.0f);
		var deckOffest = 0.5f;
		foreach(var hand in hands)
		{
			var cardList = hand.Value;
			cardList.Sort((a, b) => Sort(inGameController.gameInfo, deckIndex, a, b));
			for(int card = 0; card < cardList.Count; card++)
			{
				var cardModel = CreateCardModel(inGameController.gameInfo, deckIndex, cardList[card]);
				var pickPlayer = inGameController.GetPlayer(hand.Key);
				var cardPos = new Vector3(-cardOffset.x * cardList.Count * 0.5f + cardOffset.x * card, 0.0f, cardOffset.z * cardModel.deckIndex + deckOffest);
				LeanTween.move(cardModel.gameObject, pickPlayer.transform.position + Quaternion.Euler(0.0f, pickPlayer.rot, 0.0f) * cardPos, 0.3f);
				LeanTween.rotateY(cardModel.gameObject, pickPlayer.rot, 0.3f);
				LeanTween.scale(cardModel.gameObject, new Vector3(handScale, 1.0f, handScale), 0.3f);
			}
		}
	}
	void ApplyDiscard(GameController inGameController)
	{
		int deckIndex = inGameController.gameInfo.GetDeckIndex(mId);
		var cardList = inGameController.gameInfo.GetCardList(deckIndex);
		for(int card = 0; card < cardList.Count; ++card)
		{
			if(inGameController.gameInfo.IsDiscard(deckIndex, card))
			{
				var cardModel = CreateCardModel(inGameController.gameInfo, deckIndex, card);
				cardModel.gameObject.SetActive(false);
			}
		}
	}
	int Sort(GameInfo inInfo, int inDeck, int inA, int inB)
	{
		var b = inInfo.GetCard(inDeck, inB);
		var a = inInfo.GetCard(inDeck, inA);
		return b.GetCardType - a.GetCardType;
	}
	CardModel CreateCardModel(GameInfo inInfo, int inDeck, int inCard)
	{
		for(int i = 0; i < mCardRoot.childCount; ++i)
		{
			var child = mCardRoot.GetChild(i);
			if(child.TryGetComponent<CardModel>(out var card))
			{
				if(card.cardIndex == inCard)
				{
					return card;
				}
			}
		}
		var pos = transform.position;
		var cardPos = new Vector3(pos.x, mRoot.transform.position.y, pos.z);
		var cardModel = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f), mCardRoot);
		cardModel.Create(inInfo, inDeck, inCard, mBackface);
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
