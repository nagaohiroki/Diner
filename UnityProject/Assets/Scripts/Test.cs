using UnityEngine;
using System.Collections.Generic;
public class LTSeqContainer
{
	List<LTSeq> seqList = new List<LTSeq>();
	public LTSeq New()
	{
		var seq = LeanTween.sequence();
		seqList.Add(seq);
		return seq;
	}
}
public class Test : MonoBehaviour
{
	[SerializeField]
	GameObject mObj;
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			var seq = LeanTween.sequence();
			seq.append(LeanTween.move(gameObject, new Vector3(1.0f, 0.0f, 0.0f), 1.0f).direction);
			seq.append(LeanTween.move(gameObject, new Vector3(1.0f, 1.0f, 0.0f), 1.0f));

			var seq1 = LeanTween.sequence();
			seq1.append(LeanTween.move(mObj, new Vector3(-1.0f, 0.0f, 0.0f), 1.0f));
			seq1.append(LeanTween.move(mObj, new Vector3(-1.0f, 1.0f, 0.0f), 1.0f));
		}
	}
}
