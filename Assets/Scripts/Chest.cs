using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
	public Animator animator;
	private PopUpController popUpController;
	private bool opened = false;
	private AudioSource sfx;

	public ItemData item;

	private void Start()
	{
		popUpController = FindAnyObjectByType<PopUpController>(FindObjectsInactive.Include);
		sfx = GetComponent<AudioSource>();
	}

	public void Interact()
	{
		if (opened) return;
		opened = true;

		popUpController?.SetupPopup(item);
		popUpController?.ShowPopUp();
		animator.SetBool("isOpen", true);
		sfx.Play();
	}
}
