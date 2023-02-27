using UnityEngine;
using TMPro;
[System.Serializable]
public class IconType
{
	[SerializeField]
	CardData.CardType mCardType;
	[SerializeField]
	CardData.BonusType mBonusType;
	[SerializeField]
	Texture mTexture;
	public CardData.CardType GetCardType => mCardType;
	public CardData.BonusType GetBonusType => mBonusType;
	public Texture GetTexture => mTexture;
}
public class Icon : MonoBehaviour
{
	[SerializeField]
	MeshRenderer mMesh;
	[SerializeField]
	TextMeshPro mText;
	[SerializeField]
	IconType[] mIcons;
	Material mCache;
	public void SetIcon(CardData.CardType inType, CardData.BonusType inBonus, int inNum)
	{
		mMesh.material.mainTexture = GetTexture(inType, inBonus);
		mCache = mMesh.material;
		SetNum(inNum);
	}
	void SetNum(int inNum)
	{
		if(inNum < 0)
		{
			mText.gameObject.SetActive(false);
			return;
		}
		mText.gameObject.SetActive(true);
		mText.text = inNum.ToString();
	}
	Texture GetTexture(CardData.CardType inType, CardData.BonusType inBonus)
	{
		foreach(var icon in mIcons)
		{
			if(icon.GetCardType == inType && icon.GetBonusType == inBonus)
			{
				return icon.GetTexture;
			}
		}
		return null;
	}
	void OnDestroy()
	{
		if(mCache != null)
		{
			Destroy(mCache);
		}
	}
}
