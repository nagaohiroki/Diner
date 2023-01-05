using UnityEngine;
using TMPro;
using UnityUtility;
public class RandomText : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI mText;
	void OnEnable()
	{
		var rand = RandomObject.GetGlobal.Range(0, 10000);
		mText.text = rand.ToString("D4");
	}
}
