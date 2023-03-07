using UnityEngine;

public class Test : MonoBehaviour
{

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			var seq = LeanTween.sequence();
			seq.append(() => Move());
			Debug.Log(LeanTween.isTweening(gameObject));
		}
		if(Input.GetKeyDown(KeyCode.W))
		{

		}
	}
	LTSeq Move()
	{
		var seq = LeanTween.sequence();
		seq.append(LeanTween.move(gameObject, new Vector3(1.0f, 0.0f, 0.0f), 1.0f));
		seq.append(LeanTween.move(gameObject, new Vector3(-1.0f, 0.0f, 0.0f), 1.0f));
		Debug.Log($"Move:{LeanTween.isTweening(gameObject)}");
		return seq;
	}
}
