using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the visual representation of the player's inventory. 
/// Spawns <see cref="InventorySlotUI"/> elements and listens for data changes to refresh specific slots.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    
    [SerializeField] private GameObject inventorySlotPrefab; // Inventory slot to spawn
    [SerializeField] private GameObject hotbarSlotPrefab; // Hotbar slot to spawn
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private Transform hotbarPanel;
    [SerializeField] private int hotbarSize;

    [Header("Selection Frame Settings")] 
    [SerializeField] private RectTransform selectionFrame;
    [SerializeField] private float lerpSpeed = 15f;
    private Vector3 _targetPosition;
    
    // Keep a list of Slot scripts we created
    private readonly List<IStorageSlot> _slots = new List<IStorageSlot>();

    private void Start()
    {
        SetupUI();

        // Subscribe to events
        InventoryManager.Instance.OnSlotUpdated += RefreshSlotUI;
        InventoryManager.Instance.OnActiveSlotIndexChanged += MoveSelectionFrame;
        
        // Initial Refresh: Sync UI with whatever data is already in the Inventory Manager (Saved data)
        for (int i = 0; i < InventoryManager.Instance.itemsList.Length; i++)
        {
            RefreshSlotUI(i);
        }

        selectionFrame.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed to prevent memory leaks
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSlotUpdated -= RefreshSlotUI;
            InventoryManager.Instance.OnActiveSlotIndexChanged -= MoveSelectionFrame;
        }
    }

    private void Update()
    {
        // Moves selection frame to target position
        selectionFrame.position = Vector3.Lerp(selectionFrame.position, _targetPosition, Time.deltaTime * lerpSpeed);
    }
    
    private void SetupUI()
    {
        for (int i = 0; i < InventoryManager.Instance.itemsList.Length; i++)
        {
            // Determine if this slot should go to hotbar or inventory
            Transform targetParent = (i < hotbarSize) ? hotbarPanel : inventoryPanel;
            
            // Instantiate the slots
            GameObject prefab = (i < hotbarSize) ? hotbarSlotPrefab : inventorySlotPrefab;
            GameObject slot = Instantiate(prefab, targetParent);
            
            if (slot.TryGetComponent(out InventorySlotUI storageSlot))
            {
                storageSlot.slotScriptIndex = i;
                _slots.Add(storageSlot);
            }
        }
    }

    private void RefreshSlotUI(int index)
    {
        if (index >= 0 && index < _slots.Count)
        {
            _slots[index].RefreshSlotUI();
        }
    }

    public void MoveSelectionFrame(int index)
    {
        // Checks if _slots[Index] is an InventorySlotUI, if true, assigns it to slotBase
        if (_slots[index] is InventorySlotUI slotBase)
        {
            _targetPosition = slotBase.transform.position;
        }
    }
}