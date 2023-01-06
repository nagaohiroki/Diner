using UnityEngine;
public class DeckModel : MonoBehaviour
{
	[SerializeField]
	GameObject mAnchor;
	[SerializeField]
	CardModel mCardModelPrefab;
	[SerializeField]
	GameObject mRoot;
	[SerializeField]
	GameObject mDeck;
	public void Draw(int inId, int inIndex,int inAnchor)
	{
		var pos = transform.position;
		var cardPos = new Vector3(pos.x, mRoot.transform.position.y, pos.z);
		var card = Instantiate(mCardModelPrefab, cardPos, Quaternion.Euler(0.0f, 0.0f, 180.0f));
		card.Create(inId, inIndex);
		var targetPos = mAnchor.transform.GetChild(inAnchor).transform;
		LeanTween.move(card.gameObject, targetPos, 0.5f)
			.setOnComplete(()=>LeanTween.rotateZ(card.gameObject, 0.0f, 1.0f));
		
	}
	public void SetNum(float inBaseScale, int inNum)
	{
		if(inNum == 0)
		{
			gameObject.SetActive(false);
		}
		gameObject.SetActive(true);
		float deckScale = inBaseScale * inNum;
		var scale = mDeck.transform.localScale;
		scale.z = deckScale;
		mDeck.transform.localScale = scale;
		var pos = mRoot.transform.position;
		pos.y = deckScale * 0.5f;
		mRoot.transform.position = pos;
	}
}
