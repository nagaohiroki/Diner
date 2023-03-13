using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BackMatreial
{
	public CardFbx.BackType type;
	public Material material;
}
public class CardFbx : MonoBehaviour
{
	public enum BackType
	{
		Food,
		Cook,
		Bonus,
	}
	public enum MaterialType
	{
		Side,
		Face,
		Back,
	}
	[SerializeField]
	List<BackMatreial> mBack;
	[SerializeField]
	MeshRenderer mMesh;
	[SerializeField]
	float mBaseScale = 0.1f;
	public Vector3 GetTopPosition => transform.position + Vector3.up * 0.02f * mMesh.transform.localScale.z;
	Material[] mMaterials;
	public void SetMatrial(BackType inBack, MaterialType inMaterialType = MaterialType.Back)
	{
		var mats = mMesh.materials;
		mats[(int)inMaterialType] = GetBackMaterial(inBack);
		mMesh.materials = mats;
		mMaterials = mMesh.materials;
	}
	public void SetNum(int inNum)
	{
		if(inNum <= 0)
		{
			gameObject.SetActive(false);
			return;
		}
		gameObject.SetActive(true);
		var scale = mMesh.gameObject.transform.localScale;
		scale.z = inNum * mBaseScale;
		mMesh.gameObject.transform.localScale = scale;
	}
	Material GetBackMaterial(BackType inBack)
	{
		foreach(var back in mBack)
		{
			if(back.type == inBack)
			{
				return back.material;
			}
		}
		return null;
	}
	void OnDestroy()
	{
		if(mMaterials != null)
		{
			foreach(var mat in mMaterials)
			{
				if(mat != null)
				{
					Destroy(mat);
				}
			}
		}
	}
}
