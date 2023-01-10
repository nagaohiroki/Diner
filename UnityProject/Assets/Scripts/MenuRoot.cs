using UnityEngine;
public class MenuRoot : MonoBehaviour
{
	[SerializeField]
	MenuResult mMenuResult;
	public void Switch(GameObject inActive)
	{
		int count = transform.childCount;
		for(int i = 0; i < count; ++i)
		{
			var child = transform.GetChild(i);
			child.gameObject.SetActive(child.gameObject == inActive);
		}
	}
	public void Result(Player inWinner)
	{
		mMenuResult.SetText($"{inWinner.name} is Win!!");
		Switch(mMenuResult.gameObject);
	}
}
