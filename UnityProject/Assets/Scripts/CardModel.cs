using UnityEngine;
using TMPro;
public class CardModel : MonoBehaviour
{
	[SerializeField]
	TextMeshPro mText;
	public int supplyIndex { set; get; }
	public int cardIndex { private set; get; }
	public int deckIndex { private set; get; }
	public void Create(GameInfo inInfo, int inDeck, int inCard)
	{
		cardIndex = inCard;
		deckIndex = inDeck;
		var card = inInfo.GetCard(inDeck, inCard);
		mText.text = card.GetId;
		name = card.GetId;
		supplyIndex = -1;
	}
}
