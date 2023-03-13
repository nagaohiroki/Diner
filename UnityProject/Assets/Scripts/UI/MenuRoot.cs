using UnityEngine;
using UnityEngine.Events;
public class MenuRoot : MonoBehaviour
{
	[SerializeField]
	MenuDialog mMenuDialogPrefab;
	public void Apply(GameController inGameController)
	{
		GetComponentInChildren<MenuQuit>(true).Apply(inGameController);
	}
	public void Switch(GameObject inActive)
	{
		int count = transform.childCount;
		for(int i = 0; i < count; ++i)
		{
			var child = transform.GetChild(i);
			child.gameObject.SetActive(child.gameObject == inActive);
		}
	}
	public T SwitchMenu<T>() where T : MonoBehaviour
	{
		var menu = GetComponentInChildren<T>(true);
		Switch(menu.gameObject);
		return menu;
	}
	public MenuDialog CreateDialog(float inTime = -1.0f, UnityAction inOnEndTimer = null)
	{
		var menu = Instantiate(mMenuDialogPrefab);
		menu.SetTimer(inTime, inOnEndTimer);
		var rect = menu.GetComponent<RectTransform>();
		var parent = GetComponent<RectTransform>();
		rect.SetParent(parent, false);
		Switch(menu.gameObject);
		return menu;
	}
	public void QuitApplication()
	{
		Application.Quit();
	}
}
