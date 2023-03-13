using UnityEngine;
using TMPro;
public class MenuJoin : MonoBehaviour
{
	[SerializeField]
	TMP_InputField mPassword;
	public string GetPassword => mPassword.text;
	public void OnValueChanged()
	{
		var text = mPassword.text;
		mPassword.text = text.ToUpper();
	}
}
