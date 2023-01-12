using UnityEngine;
using TMPro;
using UnityUtility;
public class MenuCreate : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI mPassword;
	public ConnectionData CreateConnectData()
	{
		return new ConnectionData { password = mPassword.text };
	}
	void OnEnable()
	{
		mPassword.text = RandomObject.GetGlobal.Range(0, 10000).ToString("D4");
	}
}
