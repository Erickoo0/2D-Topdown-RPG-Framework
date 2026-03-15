using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
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
        if (itemInSlot == null || itemInSlot.Data == null || itemInSlot.Data.itemObject == null) return;
        
        // Spawn the Item Object
        _currentActiveItem = Instantiate(itemInSlot.Data.itemObject, transform);
        
        // Reset position
        _currentActiveItem.transform.localPosition = Vector3.zero;
        _currentActiveItem.transform.localRotation = Quaternion.identity;
        
        // 5. Disable Collision (Prevents picking up the item you are holding)
        if (_currentActiveItem.TryGetComponent(out Collider2D collision))
        {
            collision.enabled = false;
        }
        
        // Initialize the active items sprite and data
        if (_currentActiveItem.TryGetComponent(out ItemObject itemObjectScript))
        {
            itemObjectScript.InitializeItem(itemInSlot);
        }
    }

    private void TryUseActiveItem()
    {
        // Checks if the current active item has an IUsable interface and activates its Use()
        if (_currentActiveItem != null && _currentActiveItem.TryGetComponent(out IUsable useableItem))
        {
            useableItem.Use();
            InventoryManager.Instance.RemoveItems(_currentActiveSlotIndex);
        }
        else
        {
            Debug.unityLogger.Log("This item has no IUsable Interface");
        }
    }
}
