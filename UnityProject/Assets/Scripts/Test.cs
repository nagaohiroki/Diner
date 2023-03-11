using UnityEngine;
public class Test : MonoBehaviour
{
	[SerializeField]
	GameObject mObj;
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			LeanTween.addListener(0, ev => Debug.Log($"{ev.id}"));
			LeanTween.dispatchEvent(0);
		}
	}
}
