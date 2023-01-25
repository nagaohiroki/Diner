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
	float mTimer;
	UnityAction mOnEndTimer;
	public void SetTimer(float inTimer, UnityAction inOnEndTimer)
	{
		mTimer = inTimer;
		mOnEndTimer = inOnEndTimer;
	}
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
	void DestroyTimer()
	{
		if(mTimer < 0.0f)
		{
			return;
		}
		mTimer -= Time.unscaledDeltaTime;
		if(mTimer > 0.0f)
		{
			return;
		}
		mTimer = 0.0f;
		if(mOnEndTimer != null)
		{
			mOnEndTimer();
		}
		Destroy(gameObject);
	}
	void Update()
	{
		DestroyTimer();
	}
}
