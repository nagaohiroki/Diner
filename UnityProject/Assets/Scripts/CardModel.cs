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
	public int supplyIndex { set; get; }
	public int cardIndex { private set; get; }
	public int deckIndex { private set; get; }
	Material mCache;
	public void Create(GameInfo inInfo, int inDeck, int inCard, Material inMaterial)
	{
		cardIndex = inCard;
		deckIndex = inDeck;
		var card = inInfo.GetCard(inDeck, inCard);
		name = card.GetId;
		supplyIndex = -1;
		mBackfaceMesh.material = inMaterial;
		mCache = mBackfaceMesh.material;
		ApplyCardData(card);
	}
	public void ToSupply(Vector3 inPos, int inSupply, float inPitch)
	{
		if(inSupply == supplyIndex)
		{
			return;
		}
		mPickSE.pitch = inPitch;
		mPickSE.Play();
		var lt = LeanTween.move(gameObject, inPos, 0.3f);
		Open(lt);
		supplyIndex = inSupply;
	}
	public LTDescr Open(LTDescr lt)
	{
		if(!Mathf.Approximately(transform.eulerAngles.z, 180.0f))
		{
			return null;
		}
		return lt.setOnComplete(() => LeanTween.moveY(gameObject, 0.5f, 0.1f)
		.setOnComplete(() => LeanTween.rotateZ(gameObject, 0.0f, 0.1f)
		.setOnComplete(() => LeanTween.moveY(gameObject, 0.0f, 0.1f))));
	}
	void ApplyCardData(CardData inCardData)
	{
		if(inCardData == null)
		{
			return;
		}
		if(inCardData.GetCardType != CardData.CardType.Cooking)
		{
			var icon = Instantiate(mIconPrefab);
			icon.SetIcon(inCardData.GetCardType, CardData.BonusType.None);
			icon.transform.SetParent(mFoodAnchor, false);
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
		icon.SetIcon(inCardType, inBonusType);
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
