﻿using UnityEngine;
using TMPro;
public class CardModel : MonoBehaviour
{
	[SerializeField]
	TextMeshPro mText;
	public int ancherIndex { set; get; }
	public int cardIndex { private set; get; }
	public int deckIndex { private set; get; }
	public void Create(GameInfo inInfo, int inDeck, int inCard)
	{
		cardIndex = inCard;
		deckIndex = inDeck;
		var id = inInfo.GetCardList(inDeck)[inCard];
		mText.text = id.ToString();
		ancherIndex = -1;
	}
}
