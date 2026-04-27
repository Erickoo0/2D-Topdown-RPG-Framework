using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;
using TMPro;

/// <summary>
/// A centralized manager that handles drag-and-drop logic for any <see cref="IStorageSlot"/>.
/// </summary>
public class ItemDragManager : MonoBehaviour
{
    public static ItemDragManager Instance { get; private set; }

    [Header("References")] 
    [SerializeField] private Image ghostIcon; 
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
            Debug.unityLogger.Log("Multiple DragManagers detected. Disabling script.");
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

        if (_sourceSlot != null && _sourceSlot.itemInstance != null)
        {
            // Set the text
            ghostName.text = _sourceSlot.itemInstance.DataSo.ItemName;
            ghostStack.text = _sourceSlot.itemInstance.stackSize.ToString();
            _ghostIconRect.position = _currentMousePosition;
            
            // Hide item from source slot to "pick it up"
            ToggleGhost(true);
        }
        
        ghostIcon.transform.SetAsLastSibling();
    }

    private void WhileDragging()
    {
        if (!ghostIcon.enabled) return;
        
        _ghostIconRect.position = _currentMousePosition;
        if (_sourceSlot.itemInstance.DataSo.ItemIcon == null) return; 
        
        ghostIcon.sprite = GlobalHelper.GetAnimatedSprite(_sourceSlot.itemInstance.DataSo);
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
            _sourceSlot.SetDraggingState(toggle); // Tell the slot to hide its UI
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
