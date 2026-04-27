using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionDetecter : MonoBehaviour
{
    [SerializeField] private GameObject interactIcon;

    private List<IInteractable> _interactablesInRange = new List<IInteractable>();
    private IInteractable _interactableTarget;

    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        // 1. Clean the list of objects that are no longer interactable
        _interactablesInRange.RemoveAll(i => i == null || !i.CanInteract());

        if (_interactablesInRange.Count == 0)
        {
            _interactableTarget = null;
            interactIcon.SetActive(false);
            return;
        }
        
        // 2. Set the target to the closest target
        _interactableTarget = _interactablesInRange
            .OrderBy(i => Vector2.Distance(transform.position, ((MonoBehaviour)i).transform.position))
            .FirstOrDefault();    
        
        interactIcon.SetActive(true);
    }
    
    //
    
    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (!_interactablesInRange.Contains(interactable))
            {
                _interactablesInRange.Add(interactable);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            _interactablesInRange.Remove(interactable);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        // 1. Check and Trigger Interaction
        if (_interactableTarget != null && _interactableTarget.CanInteract())
        {
            PlayerController playerController = GetComponent<PlayerController>();
            _interactableTarget.Interact(playerController);
        }
        
        // 2. Item Use
        else if (PlayerEquipment.Instance != null)
        {
            PlayerEquipment.Instance.TryUseActiveItem();
        }
    }
}
