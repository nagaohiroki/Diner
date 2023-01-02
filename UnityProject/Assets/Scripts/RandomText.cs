using UnityEngine;
using TMPro;
public class RandomText : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI mText;
	void OnEnable()
	{
		var rand = Random.Range(0, 10000);
		mText.text = rand.ToString("D4");
	}
}
