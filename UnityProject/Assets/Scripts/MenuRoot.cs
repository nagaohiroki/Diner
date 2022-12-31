using UnityEngine;
public class MenuRoot : MonoBehaviour
{
	public void Switch(GameObject inActive)
	{
		int count = transform.childCount;
		for(int i = 0; i < count; ++i)
		{
			var child = transform.GetChild(i);
			child.gameObject.SetActive(child.gameObject == inActive);
		}
	}
}
