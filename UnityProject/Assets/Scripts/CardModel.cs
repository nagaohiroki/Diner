using UnityEngine;
using TMPro;
public class CardModel : MonoBehaviour
{
	[SerializeField]
	TextMeshPro mText;
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
		mText.text = card.GetId;
		name = card.GetId;
		supplyIndex = -1;
		mBackfaceMesh.material = inMaterial;
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
