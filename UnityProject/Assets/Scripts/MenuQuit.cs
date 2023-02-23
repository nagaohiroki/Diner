using UnityEngine;
using TMPro;
public class MenuQuit : MonoBehaviour
{
	[SerializeField]
	GameObject[] mHostButtons;
	[SerializeField]
	TextMeshProUGUI mPasswordText;
	[SerializeField]
	Transform mUserRoot;
	const string mDefaultText = "Password";
	public string password { private get; set; }
	public void TogglePassword()
	{
		mPasswordText.text = mPasswordText.text == mDefaultText ? password : mDefaultText;
	}
	void OnEnable()
	{
		Clear();
	}
	public void SetActiveHostButton(bool inActive)
	{
		foreach(var button in mHostButtons)
		{
			button.SetActive(inActive);
		}
	}
	void Clear()
	{
		mPasswordText.text = mDefaultText;
		for(int i = 0; i < mUserRoot.childCount; ++i)
		{
			var child = mUserRoot.GetChild(i);
			if(child.TryGetComponent<TextMeshProUGUI>(out var text))
			{
				text.text = null;
			}
		}
	}
	public void Apply(GameController inGameController)
	{
		for(int i = 0; i < mUserRoot.childCount; ++i)
		{
			var child = mUserRoot.GetChild(i);
			var players = inGameController.gameInfo.GetPlayerInfos;
			if(i >= players.Count)
			{
				child.gameObject.SetActive(false);
				continue;
			}
			if(!child.TryGetComponent<TextMeshProUGUI>(out var text))
			{
				continue;
			}
			child.gameObject.SetActive(true);
			text.color = inGameController.gameInfo.GetCurrentTurnPlayer == players[i].id ? Color.black : Color.gray;
			text.text = PlayerScore(inGameController, i);
		}
	}
	string PlayerScore(GameController inGameController, int inIndex)
	{
		var player = inGameController.gameInfo.GetPlayerInfos[inIndex];
		var playerName = inGameController.GetPlayer(player.id).name;
		int max = inGameController.GetData.GetWinPoint;
		return $"{playerName}  <align=right>{player.GetPoint}/{max} : coin:{player.coin}</align>";
	}
}
