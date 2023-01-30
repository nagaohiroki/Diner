using System.Collections.Generic;
using UnityEngine;
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
