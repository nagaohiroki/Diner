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
	MeshRenderer mBackfaceMesh;
	[SerializeField]
	TextMeshPro mCardName;
	[SerializeField]
	Vector3 mIconOffset = new Vector3(0.7f, 0.0f, 1.1f);
	[SerializeField]
	AudioSource mPickSE;
	public CardInfo cardInfo{set;get;}
	Material mCache;
	public bool IsSame(CardInfo inCard) => cardInfo.IsSame(inCard);
	public void Create(CardInfo inCard, Material inMaterial)
	{
		cardInfo = inCard;
		var card = inCard.cardData;
		name = card.GetId;
		mBackfaceMesh.material = inMaterial;
		mCache = mBackfaceMesh.material;
		LayoutIcon(card);
	}
	public void PlaySE(float inPitch)
	{
		mPickSE.pitch = inPitch;
		mPickSE.Play();
	}
	public LTDescr Open(LTDescr lt)
	{
		if(!Mathf.Approximately(transform.eulerAngles.z, 180.0f))
		{
			return null;
		}
		PlaySE(1.0f);
		return lt.setOnComplete(() => LeanTween.moveY(gameObject, 0.5f, 0.1f)
		.setOnComplete(() => LeanTween.rotateZ(gameObject, 0.0f, 0.1f)
		.setOnComplete(() => LeanTween.moveY(gameObject, 0.0f, 0.1f))));
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
		for(int i = 0; i < inCardData.GetMoney; ++i)
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
	void OnDestroy()
	{
		if(mCache != null)
		{
			Destroy(mCache);
		}
	}
}
