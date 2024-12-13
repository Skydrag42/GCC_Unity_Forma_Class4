using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpController : MonoBehaviour
{
	public GameObject popUpObject;
	public Image popUpIcon;
	public TMP_Text itemName;

	public void SetupPopup(ItemData item)
	{
		popUpIcon.sprite = item.icon;
		itemName.text = "You found a " + item.itemName + "!";
	}

	public void ShowPopUp()
	{
		PlayerController.Instance?.DeactivatePlayer();
		popUpObject.SetActive(true);
	}

	public void HidePopUp()
	{
		PlayerController.Instance?.ActivatePlayer();
		popUpObject.SetActive(false);
	}
}