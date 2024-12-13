using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
	public Animator animator;
	private PopUpController popUpController;
	private bool opened = false;

	public ItemData item;

	private void Start()
	{
		popUpController = FindAnyObjectByType<PopUpController>(FindObjectsInactive.Include);
	}

	public void Interact()
	{
		if (opened) return;
		opened = true;

		popUpController?.SetupPopup(item);
		popUpController?.ShowPopUp();
		animator.SetBool("isOpen", true);
	}
}
