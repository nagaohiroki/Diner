using UnityEngine;
public class MenuLoading : MonoBehaviour
{
	[SerializeField]
	RectTransform mTrans;
	float mPower = 1.0f;
	float mSpeed = 1.0f;
	float mAccel;
	void Start()
	{
		mAccel = 0.0f;
	}
	void Update()
	{
		mAccel += Time.deltaTime * mSpeed;
		var euler = mTrans.eulerAngles;
		euler.z += mPower * mAccel;
		mTrans.eulerAngles = euler;
	}
}
