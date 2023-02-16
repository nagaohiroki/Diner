using UnityEngine;
using UnityEngine.UI;
public class MenuCreate : MonoBehaviour
{
	[SerializeField]
	Toggle mIsBonus;
	[SerializeField]
	Toggle mIsCoin;
	public RuleData GetRule => new RuleData
	{
		isBonus = mIsBonus.isOn,
		isCoin = mIsCoin.isOn
	};
}
