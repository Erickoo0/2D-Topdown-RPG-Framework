using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// A centralized manager that handles drag-and-drop logic for any <see cref="IStorageSlot"/>.
/// </summary>
public class DragManager : MonoBehaviour
{
    public static DragManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Image ghostIcon; // A single image that follows the player
    [SerializeField] private TMP_Text ghostName;
    [SerializeField] private TMP_Text ghostStack;
        
    private IStorageSlot _sourceSlot;
    private Vector2 _currentMousePosition;
    private RectTransform _ghostIconRect;
    
    // Raycast Variables
    private PointerEventData _eventData;
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _eventData = new PointerEventData(EventSystem.current);

        _ghostIconRect = ghostIcon.GetComponent<RectTransform>();
        ToggleGhost(false);
    }

    private void Update()
    {
        if (Pointer.current == null) return;

        // Updates mouse position
        _currentMousePosition = Pointer.current.position.ReadValue();
        
        // Detects Input
        if (Pointer.current.press.wasPressedThisFrame) StartDrag();
        if (Pointer.current.press.isPressed) WhileDragging();
        if (Pointer.current.press.wasReleasedThisFrame) EndDrag();
    }

    private void StartDrag()
    {
        _sourceSlot = GetSlotUnderMouse();

        if (_sourceSlot != null && _sourceSlot.Item != null)
        {
            // Update visuals from source data
            ghostIcon.sprite = _sourceSlot.Item.Data.itemIcon;
            ghostName.text = _sourceSlot.Item.Data.itemName;
            ghostStack.text = _sourceSlot.Item.stackSize.ToString();
            ToggleGhost(true);
            _ghostIconRect.position = _currentMousePosition;
            
            // Hide item from source slot to "pick it up"
            _sourceSlot.SetVisibility(false);
        }
    }

    private void WhileDragging()
    {
        if (ghostIcon.enabled)
        {
            _ghostIconRect.position = _currentMousePosition;
        }
    }

    private void EndDrag()
    {
        if (_sourceSlot == null) return;

        //Find the target slot
        IStorageSlot targetSlot = GetSlotUnderMouse();

        // Swap Items Logic
        if (targetSlot != null && targetSlot != _sourceSlot)
        {
            InventoryManager.Instance.SwapItems(_sourceSlot.Index, targetSlot.Index);
        }
        
        // Drop Items Logic
        if (targetSlot == null)
        {
            // Converts mouse coordinates into 2D world coordinates
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(_currentMousePosition);
            mouseWorldPosition.z = 0;
            InventoryManager.Instance.DropItems(_sourceSlot.Index, mouseWorldPosition);
        }
        
        // Clean up
        ToggleGhost(false);
        _sourceSlot.SetVisibility(true);
        _sourceSlot = null;
 
    }

    private void ToggleGhost(bool toggle)
    {
        _ghostIconRect.gameObject.SetActive(toggle);
        ghostIcon.enabled = toggle;
        ghostName.enabled = toggle;
        ghostStack.enabled = toggle;

        if (_sourceSlot != null)
        { 
            _sourceSlot.SetVisibility(!toggle);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IStorageSlot GetSlotUnderMouse()
    {
        _eventData.position = _currentMousePosition;
        _raycastResults.Clear(); // Clear the old results instead of making a new list
    
        EventSystem.current.RaycastAll(_eventData, _raycastResults);

        foreach (var result in _raycastResults)
        {
            IStorageSlot slot = result.gameObject.GetComponentInParent<IStorageSlot>();
            if (slot != null) return slot;
        }
        return null;
    }
}
