using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Equipment Settings")]
    [Tooltip("The transform where the instantiated item will be parented")]
    [SerializeField] private Transform parentTransform;
    
    private GameObject _currentActiveItem;
    private int _currentActiveSlotIndex = -1;
    
    private void Start()
    {
        // Listens for when the player switches their selection
        InventoryManager.Instance.OnActiveSlotIndexChanged += SetActiveSlotIndex;
        // Listens for when data inside any slot changes (Add, Drop, Swap)
        InventoryManager.Instance.OnSlotUpdated += SetSlotData;
        // Listens for when Use Item Key is pressed
        HotbarManager.Instance.OnUseItemInput += TryUseActiveItem;
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnActiveSlotIndexChanged -= SetActiveSlotIndex;
            InventoryManager.Instance.OnSlotUpdated -= SetSlotData;
        }

        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.OnUseItemInput -= TryUseActiveItem;
        }
    }
    
    private void SetActiveSlotIndex(int index)
    {
        parentTransform.gameObject.SetActive(true);
        _currentActiveSlotIndex = index;
        // Find the Item Data from slot index
        ItemInstance itemInSlot = InventoryManager.Instance.itemsList[_currentActiveSlotIndex];
        // Set the active item to the one from slot index
        SetActiveItem(itemInSlot);
    }

    private void SetSlotData(int index)
    {
        // Only update if the slot modified matches active slot
        if (index == _currentActiveSlotIndex)
        {
            ItemInstance itemInSlot = InventoryManager.Instance.itemsList[_currentActiveSlotIndex];
            SetActiveItem(itemInSlot);
        }
    }

    private void SetActiveItem(ItemInstance itemInSlot)
    {
        // Destroy the old active item if it exists
        if (_currentActiveItem != null) Destroy(_currentActiveItem);
        
        // Safety Check: If slot is empty or null
        if (itemInSlot == null || itemInSlot.Data == null || itemInSlot.Data.ItemObject == null) return;
        
        // Spawn the Item Object
        _currentActiveItem = Instantiate(itemInSlot.Data.ItemObject, parentTransform);
        
        // Reset position
        _currentActiveItem.transform.localPosition = Vector3.zero;
        _currentActiveItem.transform.localRotation = Quaternion.identity;
        
        // Initialize the active items sprite and data
        if (_currentActiveItem.TryGetComponent(out ItemObject itemObjectScript))
        {
            itemObjectScript.SetItemObject(itemInSlot, null, false);
        }
    }

    private void TryUseActiveItem()
    {
        if (_currentActiveSlotIndex < 0) return;

        ItemInstance activeItem = InventoryManager.Instance.itemsList[_currentActiveSlotIndex];
        
        if (activeItem == null || activeItem.Data == null) return;

        if (activeItem.Data.IsUsable == true)
        {
            bool wasUsed = activeItem.Data.Use(gameObject, activeItem);
            if (wasUsed)
            {
                InventoryManager.Instance.RemoveItems(_currentActiveSlotIndex);
            }
            return;
        }
    }
}
