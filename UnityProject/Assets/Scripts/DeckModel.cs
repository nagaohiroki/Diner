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
	CardModel mCardModelPrefab;
	[SerializeField]
	GameObject mDeck;
	[SerializeField]
	MeshRenderer mBackfaceMesh;
	Material mCache;
	public string GetId => mId;
	public void Layout(GameInfo inGameInfo, Transform inCardRoot)
	{
		var deck = inGameInfo.GetDeck(mId);
		var deckIndex = inGameInfo.GetDeckIndex(mId);
		for(int supply = 0; supply < deck.supply.Count; ++supply)
		{
			var card = deck.supply[supply];
			var pos = transform.position + mSupplyOffset * (supply + 1);
			var cardModel = CreateCardModel(card, inCardRoot);
			cardModel.Open(LeanTween.move(cardModel.gameObject, pos, 0.3f));
		}
		SetDeckSize(0.002f, deck.GetCardList.Count);
	}
	public CardModel CreateCardModel(CardInfo inCard, Transform inParent)
	{
		for(int i = 0; i < inParent.childCount; ++i)
		{
			var child = inParent.GetChild(i);
			if(child.TryGetComponent<CardModel>(out var card))
			{
				if(card.IsSame(inCard))
				{
					return card;
				}
			}
		}
		var pos = transform.position;
		var cardPos = new Vector3(pos.x, mBackfaceMesh.transform.position.y, pos.z);
		var cardModel = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f), inParent);
		cardModel.Create(inCard, mBackface);
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
