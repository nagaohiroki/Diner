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
	public void ToSupply(Vector3 inPos, int inSupply)
	{
		if(inSupply == supplyIndex)
		{
			return;
		}
		var lt = LeanTween.move(gameObject, inPos, 0.3f);
		if(supplyIndex == -1)
		{
			lt.setOnComplete(() => LeanTween.moveY(gameObject, 0.5f, 0.1f)
			.setOnComplete(() => LeanTween.rotateZ(gameObject, 0.0f, 0.1f)
			.setOnComplete(() => LeanTween.moveY(gameObject, 0.0f, 0.1f))));
		}
		supplyIndex = inSupply;
	}
	public bool Hand(int inDeck, int inCard, GameController inGameController)
	{
		var pickId = inGameController.gameInfo.GetPickPlayer(inDeck, inCard);
		if(pickId == null)
		{
			return false;
		}
		if(inGameController.gameInfo.IsDiscard(inDeck, inCard))
		{
			gameObject.SetActive(false);
			return true;
		}
		float handScale = 0.5f;
		var cardOffset = new Vector3(0.6f, 0.0f, 1.0f);
		var deckOffest = 0.5f;
		var pickPlayer = inGameController.GetPlayer(pickId);
		(int handIndex, int handMax) = inGameController.gameInfo.GetHand(inDeck, inCard, pickId);
		var cardPos = new Vector3(-cardOffset.x * handMax * 0.5f + cardOffset.x * handIndex, 0.0f, cardOffset.z * inDeck + deckOffest);
		LeanTween.move(gameObject, pickPlayer.transform.position + Quaternion.Euler(0.0f, pickPlayer.rot, 0.0f) * cardPos, 0.3f);
		LeanTween.rotateY(gameObject, pickPlayer.rot, 0.3f);
		LeanTween.scale(gameObject, new Vector3(handScale, 1.0f, handScale), 0.3f);
		return true;
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
