using UnityEngine;
using TMPro;
public class MenuQuit : MonoBehaviour
{
	[SerializeField]
	GameObject[] mHostButtons;
	[SerializeField]
	GameObject mPassword;
	[SerializeField]
	Transform mUserRoot;
	const string mDefaultText = "Password";
	public string password { private get; set; }
	public void TogglePassword(TextMeshProUGUI inText)
	{
		if(inText.text == mDefaultText)
		{
			inText.text = password;
			return;
		}
		inText.text = mDefaultText;
	}
	public void SetActiveHostButton(bool inActive)
	{
		foreach(var button in mHostButtons)
		{
			button.SetActive(inActive);
		}
	}
	public void Apply(GameController inGameController)
	{
		for(int i = 0; i < mUserRoot.childCount; ++i)
		{
			var child = mUserRoot.GetChild(i);
			var players = inGameController.gameInfo.GetTurnPlayers;
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
			text.color = inGameController.gameInfo.GetCurrentTurnPlayer == players[i] ? Color.black : Color.gray;
			text.text = PlayerScore(inGameController, i);
		}
	}
	string PlayerScore(GameController inGameController, int inIndex)
	{
		var players = inGameController.gameInfo.GetTurnPlayers;
		var id = players[inIndex];
		var player = inGameController.GetPlayer(id);
		int current = inGameController.gameInfo.GetPoint(id);
		int max = inGameController.GetData.GetWinPoint;
		return $"{player.name}  <align=right>{current}/{max}</align>";
	}
}
