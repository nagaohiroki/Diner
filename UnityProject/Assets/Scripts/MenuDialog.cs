using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class MenuDialog : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI mTextPrefab;
	[SerializeField]
	Button mButtonPrefab;
	[SerializeField]
	RectTransform mRoot;
	public TextMeshProUGUI AddText(string inText)
	{
		var text = Instantiate(mTextPrefab, mRoot);
		text.SetText(inText);
		return text;
	}
	public Button AddButton(string inText, UnityAction inAction)
	{
		var button = Instantiate(mButtonPrefab, mRoot);
		button.GetComponentInChildren<TextMeshProUGUI>().text = inText;
		button.onClick.AddListener(inAction);
		return button;
	}
	public Button AddButtonAndClose(string inText, UnityAction inAction)
	{
		var button = AddButton(inText, inAction);
		button.onClick.AddListener(() => Destroy(gameObject));
		return button;
	}
}
