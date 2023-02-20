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
	GameObject mDeck;
	[SerializeField]
	MeshRenderer mBackfaceMesh;
	Material mCache;
	Transform mCardRoot;
	public string GetId => mId;
	public void Apply(GameController inGameController, Transform inCardRoot)
	{
		if(mCardRoot == null)
		{
			var root = new GameObject(mId);
			root.transform.SetParent(inCardRoot);
			mCardRoot = root.transform;
		}
		ApplySupply(inGameController);
		ApplyDiscard(inGameController.gameInfo);
	}
	void ApplySupply(GameController inGameController)
	{
		var info = inGameController.gameInfo;
		int deckIndex = info.GetDeckIndex(mId);
		var cardList = info.GetCardList(deckIndex);
		for(int card = 0; card < cardList.Count; ++card)
		{
			int supply = info.SupplyIndex(deckIndex, card);
			if(supply == -1)
			{
				continue;
			}
			var pos = transform.position + mSupplyOffset * (supply + 1);
			var cardModel = CreateCardModel(info, deckIndex, card);
			cardModel.PlaySE(mSEPitch);
			cardModel.Open(LeanTween.move(cardModel.gameObject, pos, 0.3f));
		}
		SetDeckSize(0.002f, info.RemainingCards(deckIndex));
	}
	void ApplyDiscard(GameInfo inInfo)
	{
		int deckIndex = inInfo.GetDeckIndex(mId);
		var cardList = inInfo.GetCardList(deckIndex);
		for(int card = 0; card < cardList.Count; ++card)
		{
			if(inInfo.IsDiscard(deckIndex, card))
			{
				var cardModel = CreateCardModel(inInfo, deckIndex, card);
				cardModel.gameObject.SetActive(false);
			}
		}
	}
	public CardModel CreateCardModel(GameInfo inInfo, int inDeck, int inCard)
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
		var cardPos = new Vector3(pos.x, mBackfaceMesh.transform.position.y, pos.z);
		var cardModel = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f), mCardRoot);
		cardModel.Create(inInfo, inDeck, inCard, mBackface);
		return cardModel;
	}
	void SetDeckSize(float inBaseScale, int inNum)
	{
		if(inNum == 0)
		{
			gameObject.SetActive(false);
			return;
		}
		gameObject.SetActive(true);
		float deckScale = inBaseScale * inNum;
		var scale = mDeck.transform.localScale;
		scale.z = deckScale;
		mDeck.transform.localScale = scale;
		var pos = mDeck.transform.position;
		pos.y = deckScale * 0.5f;
		mDeck.transform.position = pos;
		var backPos = mBackfaceMesh.transform.position;
		backPos.y = deckScale;
		mBackfaceMesh.transform.position = backPos;
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
