using UnityEngine;
using TMPro;
public class CardModel : MonoBehaviour
{
	[SerializeField]
	TextMeshPro mText;
	int mIndex;
	public void Create(int id, int inIndex)
	{
		mText.text = id.ToString();
		mIndex = inIndex;
	}
}
