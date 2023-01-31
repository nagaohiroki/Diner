using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class CardCounter
{
	[SerializeField]
	CardData mCard;
	[SerializeField]
	int mNum;
	public CardData GetCard => mCard;
	public int GetNum => mNum;
	public override string ToString()
	{
		return $"{mCard} x{mNum}";
	}
	public void Set(CardData inCardData)
	{
		mCard = inCardData;
	}
}
[CreateAssetMenu]
public class DeckData : ScriptableObject
{
	[SerializeField]
	string id;
	[SerializeField]
	int supply;
	[SerializeField]
	List<CardCounter> mCard;
	public string GetId => id;
	public int GetSupply => supply;
	public List<CardData> GenerateCardList()
	{
		var list = new List<CardData>();
		foreach(var card in mCard)
		{
			for(int i = 0; i < card.GetNum; i++)
			{
				list.Add(card.GetCard);
			}
		}
		return list;
	}
	public override string ToString()
	{
		var log = $"{id}:\n";
		int total = 0;
		foreach(var card in mCard)
		{
			log += $"{card}\n";
			total += card.GetNum;
		}
		log += $"---\nTotal:{total}\n";
		return log;
	}
#if UNITY_EDITOR
	public void OnValidate()
	{
		var paths = AssetDatabase.GetAllAssetPaths();
		foreach(var path in paths)
		{
			if(!path.Contains(id))
			{
				continue;
			}
			var cardData = AssetDatabase.LoadAssetAtPath<CardData>(path);
			if(cardData == null)
			{
				continue;
			}
			var cards = mCard.Find(card => card.GetCard.GetId == cardData.GetId);
			if(cards == null)
			{
				var cardCounter = new CardCounter();
				cardCounter.Set(cardData);
				mCard.Add(cardCounter);
			}
		}
	}
#endif
}
