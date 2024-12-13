using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    private bool interactRequested = false;
    public float maxInteractionDistance = 3f;
    public LayerMask interactableLayer;

    RaycastHit hitInfo;
    private void FixedUpdate()
    {
        if (interactRequested)
        {
            interactRequested = false;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f, .5f)), out hitInfo, maxInteractionDistance, interactableLayer))
            {
                IInteractable interactable = hitInfo.collider.GetComponentInParent<IInteractable>();
                if (interactable != null) interactable.Interact();
            }
        }
    }

    public void GetInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactRequested = true;
        }
    }
}
