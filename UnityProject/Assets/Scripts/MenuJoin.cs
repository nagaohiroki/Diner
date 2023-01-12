using UnityEngine;
using TMPro;
public class MenuJoin : MonoBehaviour
{
	[SerializeField]
	TMP_InputField mPassword;
	public ConnectionData CreateConnectData()
	{
		return new ConnectionData { password = mPassword.text };
	}
}
