using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CardCounter
{
#if UNITY_EDITOR
	[SerializeField]
	string id;
#endif
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
#if UNITY_EDITOR
	public void OnValidate()
	{
		id = ToString();
	}
#endif
}
[System.Serializable]
public class DeckData
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
		foreach(var card in mCard)
		{
			card.OnValidate();
		}
	}
#endif
}
[CreateAssetMenu]
public class BattleData : ScriptableObject
{
	[SerializeField]
	int winPoint;
	[SerializeField]
	List<DeckData> deckList;
	public List<DeckData> GetDeckList => deckList;
	public int GetWinPoint => winPoint;
	public override string ToString()
	{
		var log = string.Empty;
		foreach(var deck in deckList)
		{
			log += $"{deck}\n";
		}
		return log;
	}
#if UNITY_EDITOR
	public void OnValidate()
	{
		foreach(var deck in deckList)
		{
			deck.OnValidate();
		}
	}
#endif
}
