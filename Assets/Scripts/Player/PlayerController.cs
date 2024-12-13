using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance;

	public PlayerInput playerInput;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void DeactivatePlayer()
	{
		playerInput?.DeactivateInput();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void ActivatePlayer()
	{
		playerInput?.ActivateInput();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

}