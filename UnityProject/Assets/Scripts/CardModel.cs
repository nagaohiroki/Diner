using UnityEngine;
using TMPro;
public class CardModel : MonoBehaviour
{
	[SerializeField]
	Icon mIconPrefab;
	[SerializeField]
	Transform mPointAnchor;
	[SerializeField]
	Transform mFoodAnchor;
	[SerializeField]
	Transform mCostAnchor;
	[SerializeField]
	Transform mCoinAnchor;
	[SerializeField]
	Transform mBonusAnchor;
	[SerializeField]
	CardFbx mCard;
	[SerializeField]
	TextMeshPro mCardName;
	[SerializeField]
	Vector3 mIconOffset = new Vector3(0.7f, 0.0f, 1.1f);
	[SerializeField]
	AudioSource mPickSE;
	public CardInfo cardInfo { set; get; }
	public int supply { get; set; } = -1;
	public bool IsSame(CardInfo inCard) => cardInfo.IsSame(inCard);
	public void Create(CardInfo inCard)
	{
		cardInfo = inCard;
		var card = inCard.cardData;
		name = card.GetId;
		LayoutIcon(card);
		var data = inCard.cardData;
		switch(data.GetCardType)
		{
			case CardData.CardType.Cooking:
				mCard.SetMatrial(CardFbx.BackType.Cook);
				break;
			case CardData.CardType.Bonus:
				mCard.SetMatrial(CardFbx.BackType.Bonus);
				break;
			default:
				mCard.SetMatrial(CardFbx.BackType.Food);
				break;
		}
	}
	public void PlaySE(float inPitch)
	{
		mPickSE.pitch = inPitch;
		mPickSE.Play();
	}
	public void Open(LTSeq inSeq)
	{
		if(!Mathf.Approximately(transform.eulerAngles.z, 180.0f))
		{
			return;
		}
		inSeq.append(LeanTween.moveY(gameObject, 1.0f, 0.1f).setEaseOutCubic());
		inSeq.append(LeanTween.rotateZ(gameObject, 0.0f, 0.4f).setEaseInOutBack());
		inSeq.append(LeanTween.moveY(gameObject, 0.0f, 0.1f).setEaseOutExpo());
		inSeq.append(() => PlaySE(1.0f));
	}
	public void Discard(LTSeq inSeq, CardModel inPick)
	{
		inSeq.append(LeanTween.move(gameObject, inPick.transform.position + new Vector3(0.0f, 1.0f, 0.0f), 0.5f).setEaseOutCubic());
		inSeq.append(LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEaseInOutExpo());
		inSeq.append(() => gameObject.SetActive(false));
	}
	public void CanNotPick()
	{
		var seq = LeanTween.sequence();
		seq.append(LeanTween.rotateY(gameObject, 5.0f, 0.1f).setEaseShake());
		seq.append(LeanTween.rotateY(gameObject, 0.0f, 0.1f).setEaseShake());
	}
	void LayoutIcon(CardData inCardData)
	{
		if(inCardData == null)
		{
			return;
		}
		if(inCardData.GetCardType != CardData.CardType.Cooking && inCardData.GetCardType != CardData.CardType.Bonus)
		{
			CreateIcon(mFoodAnchor, Vector3.zero, inCardData.GetCardType, CardData.BonusType.None);
			CreateIcon(mBonusAnchor, Vector3.zero, inCardData.GetCardType, CardData.BonusType.None);
			mCardName.gameObject.SetActive(false);
			return;
		}
		mCardName.gameObject.SetActive(true);
		mCardName.text = inCardData.GetId;
		if(inCardData.GetBonusType != CardData.BonusType.None)
		{
			CreateIcon(mBonusAnchor, Vector3.zero, CardData.CardType.Cooking, inCardData.GetBonusType);
		}
		for(int i = 0; i < inCardData.GetPoint; ++i)
		{
			var pos = new Vector3((i) * mIconOffset.x, 0.0f, 0.0f);
			CreateIcon(mPointAnchor, pos, CardData.CardType.Cooking, CardData.BonusType.None);
		}
		for(int i = 0; i < inCardData.GetCoin; ++i)
		{
			var pos = new Vector3(i * mIconOffset.x, 0.0f, 0.0f);
			CreateIcon(mCoinAnchor, pos, CardData.CardType.Coin, CardData.BonusType.None);
		}
		for(int costType = 0; costType < inCardData.GetBonusCosts.Count; ++costType)
		{
			var cost = inCardData.GetBonusCosts[costType];
			for(int i = 0; i < cost.GetNum; ++i)
			{
				var pos = new Vector3(i * mIconOffset.x, 0.0f, -costType * mIconOffset.z);
				CreateIcon(mCostAnchor, pos, CardData.CardType.Cooking, cost.GetBonusType);
			}
		}
		for(int costType = 0; costType < inCardData.GetCost.Count; ++costType)
		{
			var cost = inCardData.GetCost[costType];
			for(int i = 0; i < cost.GetNum; ++i)
			{
				var pos = new Vector3(i * mIconOffset.x, 0.0f, -costType * mIconOffset.z);
				CreateIcon(mCostAnchor, pos, cost.GetCostType, CardData.BonusType.None);
			}
		}
	}
	void CreateIcon(Transform inParent, Vector3 inPos, CardData.CardType inCardType, CardData.BonusType inBonusType)
	{
		var icon = Instantiate(mIconPrefab);
		icon.SetIcon(inCardType, inBonusType, -1);
		icon.transform.SetParent(inParent, false);
		icon.transform.localPosition = inPos;
	}
}
