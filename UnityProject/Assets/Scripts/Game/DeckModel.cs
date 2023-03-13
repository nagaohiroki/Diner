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
	[SerializeField]
	float mStartTime;
	public string GetId => mId;
	public void Layout(DeckInfo inDeck, Transform inCardRoot, float inTweenTime, System.Action inEnd)
	{
		if(gameObject.activeSelf)
		{
			LayoutSupply(inDeck, inCardRoot, inTweenTime, inEnd);
			return;
		}
		var seq = LeanTween.sequence();
		gameObject.SetActive(true);
		var pos = transform.position;
		pos.y = 10.0f;
		transform.position = pos;
		seq.append(LeanTween.moveY(gameObject, 0.0f, mStartTime).setEaseOutCubic());
		seq.append(() => LayoutSupply(inDeck, inCardRoot, inTweenTime, inEnd));
	}
	void LayoutSupply(DeckInfo inDeck, Transform inCardRoot, float inTweenTime, System.Action inEnd)
	{
		mCardFbx.SetNum(inDeck.GetCardList.Count);
		int supply = 0;
		var seqCounter = new SeqCounter(inDeck.supply.Count, inEnd);
		foreach(var card in inDeck.supply)
		{
			++supply;
			var cardModel = CreateCardModel(card, inCardRoot);
			if(cardModel.supply == supply)
			{
				seqCounter.End();
				continue;
			}
			cardModel.supply = supply;
			var pos = transform.position + mSupplyOffset * supply;
			var seq = LeanTween.sequence();
			seq.append(LeanTween.move(cardModel.gameObject, pos, inTweenTime).setEaseInOutExpo());
			cardModel.Open(seq);
			seq.append(seqCounter.End);
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
		var cardModel = Instantiate(mCardModelPrefab, mCardFbx.GetTopPosition, Quaternion.Euler(0.0f, 0.0f, 180.0f), inParent);
		cardModel.Create(inCard);
		return cardModel;
	}
	public void Clear()
	{
		gameObject.SetActive(false);
	}
	void Start()
	{
		mCardFbx.SetMatrial(mBackType, CardFbx.MaterialType.Face);
	}
}
