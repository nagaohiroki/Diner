using UnityEngine;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	string mId;
	[SerializeField]
	CardFbx.BackType mBackType;
	[SerializeField]
	Vector3 mSupplyOffset;
	[SerializeField]
	CardModel mCardModelPrefab;
	[SerializeField]
	CardFbx mCardFbx;
	public string GetId => mId;
	public void Layout(DeckInfo inDeck, Transform inCardRoot, float inTweenTime)
	{
		mCardFbx.SetNum(inDeck.GetCardList.Count);
		int supply = 0;
		foreach(var card in inDeck.supply)
		{
			++supply;
			var pos = transform.position + mSupplyOffset * supply;
			var cardModel = CreateCardModel(card, inCardRoot);
			cardModel.Open(LeanTween.move(cardModel.gameObject, pos, inTweenTime));
		}
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
		var cardPos = new Vector3(pos.x, mCardFbx.transform.position.y + 0.1f, pos.z);
		var cardModel = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f), inParent);
		cardModel.Create(inCard);
		return cardModel;
	}
	void Start()
	{
		mCardFbx.SetMatrial(mBackType, CardFbx.MaterialType.Face);
	}
}
