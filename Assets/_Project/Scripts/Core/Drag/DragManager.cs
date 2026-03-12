using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;

/// <summary>
/// A centralized manager that handles drag-and-drop logic for any <see cref="IStorageSlot"/>.
/// </summary>
public class DragManager : MonoBehaviour
{
    public static DragManager Instance { get; private set; }

    private Image _ghostIcon; // A single image that follows the player
    private RectTransform _ghostIconRect;
    private IStorageSlot _sourceSlot;
    private Vector2 _currentMousePosition;
    
    private PointerEventData _eventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _eventData = new PointerEventData(EventSystem.current);

        // Get the Ghost Icon image from Drag Manager's children
        _ghostIcon = GetComponentInChildren<Image>();
        _ghostIconRect = _ghostIcon.GetComponent<RectTransform>();
        if (_ghostIcon != null)
        {
            _ghostIcon.enabled = false;
            _ghostIcon.raycastTarget = false;
        }
    }

    private void Update()
    {
        if (Pointer.current == null) return;
        
        // Updates mouse position
        _currentMousePosition = Pointer.current.position.ReadValue();
        
        // Detects Input
        if (Pointer.current.press.wasPressedThisFrame)
        {
            StartDrag();
        }

        if (Pointer.current.press.isPressed)
        {
            WhileDragging();
        }

        if (Pointer.current.press.wasReleasedThisFrame)
        {
            EndDrag();
        }
    }

    private void StartDrag()
    {
        _sourceSlot = GetSlotUnderMouse();

        if (_sourceSlot != null && _sourceSlot.Item != null)
        {
            // Takes the item from source slot and enables ghost icon
            _ghostIcon.sprite = _sourceSlot.Item.itemIcon;
            _ghostIcon.enabled = true;
            _ghostIcon.transform.position = _currentMousePosition;
        }
    }

    private void WhileDragging()
    {
        if (_ghostIcon.enabled)
        {
            _ghostIconRect.position = _currentMousePosition;
        }
    }

    private void EndDrag()
    {
        if (_sourceSlot == null) return;

        //Find the target slot
        IStorageSlot targetSlot = GetSlotUnderMouse();

        // Checks if dropped on a valid slot
        if (targetSlot != null && targetSlot != _sourceSlot)
        {
            // Swap items
            InventoryManager.Instance.SwapItems(_sourceSlot.Index, targetSlot.Index);
        }
        
        // Clean up
        _ghostIcon.sprite = null;
        _ghostIcon.enabled = false;
        _sourceSlot = null;
    }
    
    private IStorageSlot GetSlotUnderMouse()
    {
        _eventData.position = _currentMousePosition;
        _raycastResults.Clear(); // Clear the old results instead of making a new list
    
        EventSystem.current.RaycastAll(_eventData, _raycastResults);

        foreach (var result in _raycastResults)
        {
            // Instead of GetComponentInParent
            if (result.gameObject.TryGetComponent(out IStorageSlot slot))
            {
                return slot;
            }
        }
        return null;
    }
}
