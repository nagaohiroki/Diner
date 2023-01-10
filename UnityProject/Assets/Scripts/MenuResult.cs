using UnityEngine;
using TMPro;
public class MenuResult : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI mText;
	public void SetText(string inText)
	{
		mText.text = inText;
	}
}
