using UnityEngine;
[System.Serializable]
public class IconType
{
	[SerializeField]
	CardData.CardType mCardType;
	[SerializeField]
	Texture mTexture;
	public CardData.CardType GetCardType => mCardType;
	public Texture GetTexture => mTexture;
}
public class Icon : MonoBehaviour
{
	[SerializeField]
	MeshRenderer mMesh;
	[SerializeField]
	IconType[] mIcons;
	Material mCache;
	public void SetIcon(CardData.CardType inType)
	{
		mMesh.material.mainTexture = GetTexture(inType);
		mCache = mMesh.material;
	}
	Texture GetTexture(CardData.CardType inType)
	{
		foreach(var icon in mIcons)
		{
			if(icon.GetCardType == inType)
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
