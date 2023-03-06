using UnityEngine;

public class Test : MonoBehaviour
{
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			var seq = LeanTween.sequence();
			seq.append(LeanTween.rotateY(gameObject, 10.0f, 0.2f).setEaseShake());
			seq.append(LeanTween.rotateY(gameObject, 0.0f, 0.1f).setEaseShake());
		}
		if(Input.GetKeyDown(KeyCode.W))
		{
			LeanTween.move(gameObject, new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
		}
	}
}
