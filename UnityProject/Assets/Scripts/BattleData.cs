using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class DeckData
{
	[SerializeField]
	string id;
	[SerializeField]
	List<CardData> mCardData;
	public List<CardData> GenerateCardList()
	{
		var list = new List<CardData>();
		foreach(var card in mCardData)
		{
			for(int i = 0; i < card.GetNum; i++)
			{
				list.Add(card);
			}
		}
		return list;
	}
	public override string ToString()
	{
		var log = $"{id}:\n";
		int total = 0;
		foreach(var card in mCardData)
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
		mCardData.Clear();
		var paths = AssetDatabase.GetAllAssetPaths();
		foreach(var path in paths)
		{
			if(!path.Contains($"{id}"))
			{
				continue;
			}
			var cardData = AssetDatabase.LoadAssetAtPath<CardData>(path);
			if(cardData != null)
			{
				mCardData.Add(cardData);
			}
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
		foreach (var deck in deckList)
		{
		    log += $"{deck}\n";
		}
		return log;
	}
#if UNITY_EDITOR
	[Multiline(30)]
	public string Info;
	public void OnValidate()
	{
		Info = ToString();
		foreach(var deck in deckList)
		{
			deck.OnValidate();
		}
	}
#endif
}
