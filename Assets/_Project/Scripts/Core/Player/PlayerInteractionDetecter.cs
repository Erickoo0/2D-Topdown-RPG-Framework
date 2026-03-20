using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines.ExtrusionShapes;

public class PlayerInteractionDetecter : MonoBehaviour
{
    [SerializeField] private GameObject interactIcon;

    private IInteractable interactableInRange;

    private void Start() => interactIcon.SetActive(false);
    
    private void OnTriggerEnter2D(Collider2D other)
    {   
        // Check all objects within circle collider for an IInteractable interface, then check if we can interact with it (not opened)
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            // Set the Interactable target
            interactableInRange = interactable;
            interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check all objects for an IInteractable interface that is the TARGET, then set it to null
        if (other.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactIcon.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }
}
