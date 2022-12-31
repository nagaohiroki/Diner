using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
	[SerializeField]
	PlayerInput mInput;
	public PlayerInput GetInput => mInput;
}
