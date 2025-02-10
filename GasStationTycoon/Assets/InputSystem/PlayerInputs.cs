using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Player character Input Values")]
		public Vector2 move;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
#endif
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

#if !UNITY_IOS || !UNITY_ANDROID
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
#endif
	}
}