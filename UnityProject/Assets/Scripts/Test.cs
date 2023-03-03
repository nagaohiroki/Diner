using UnityEngine;

public class Test : MonoBehaviour
{
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			var seq = LeanTween.sequence();
			seq.append(LeanTween.move(gameObject, new Vector3(-3.0f, 3.0f, 0.0f), 1.0f));
			seq.append(LeanTween.move(gameObject, new Vector3(3.0f, 3.0f, 0.0f), 1.0f));
			seq.append(LeanTween.move(gameObject, new Vector3(3.0f, -3.0f, 0.0f), 1.0f));
			seq.append(LeanTween.move(gameObject, new Vector3(-3.0f, -3.0f, 0.0f), 1.0f));
		}
		if(Input.GetKeyDown(KeyCode.W))
		{
			LeanTween.move(gameObject, new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
		}
	}
}
