using UnityEngine;
using System.Collections.Generic;
public class Table : MonoBehaviour
{
	[SerializeField]
	List<DeckModel> mDeckModels;
	public int DeckNum => mDeckModels.Count;
	public void Apply(GameInfo inGameInfo)
	{
		for(int i = 0; i < mDeckModels.Count; ++i)
		{
			mDeckModels[i].Apply(i, inGameInfo);
		}
	}
}
