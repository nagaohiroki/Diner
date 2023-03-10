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
			LeanTween.addListener(0, ev => Debug.Log($"{ev.id}"));
			LeanTween.dispatchEvent(0);
		}
	}
}
