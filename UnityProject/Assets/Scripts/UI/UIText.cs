using UnityEngine;
using TMPro;
public class UIText : MonoBehaviour
{
	[SerializeField]
	string mTextId;
	[SerializeField]
	TextMeshProUGUI mText;
#if UNITY_EDITOR
	void OnValidate()
	{
		if(mText == null)
		{
			TryGetComponent<TextMeshProUGUI>(out mText);
		}
	}
#endif
}
