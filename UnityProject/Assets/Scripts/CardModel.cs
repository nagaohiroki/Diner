using UnityEngine;
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
	MeshRenderer mBackfaceMesh;
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
	void ApplyCardData(CardData inCardData)
	{
		if(inCardData.GetCardType != CardData.CardType.Cooking)
		{
			var icon = Instantiate(mIconPrefab);
			icon.SetIcon(inCardData.GetCardType);
			icon.transform.SetParent(mFoodAnchor, false);
			return;
		}
		for(int i = 0; i < inCardData.GetPoint; i++)
		{
			var icon = Instantiate(mIconPrefab);
			icon.SetIcon(CardData.CardType.Cooking);
			icon.transform.SetParent(mPointAnchor, false);
			icon.transform.localPosition = Vector3.right * i;
		}
		for(int costType = 0; costType < inCardData.GetCost.Count; ++costType)
		{
			var cost = inCardData.GetCost[costType];
			for(int i = 0; i < cost.GetNum; i++)
			{
				var icon = Instantiate(mIconPrefab);
				icon.SetIcon(cost.GetCostType);
				icon.transform.SetParent(mCostAnchor, false);
				icon.transform.localPosition = new Vector3(i, 0.0f, -costType);
			}
		}
	}
	void OnDestroy()
	{
		if(mCache != null)
		{
			Destroy(mCache);
		}
	}
}
