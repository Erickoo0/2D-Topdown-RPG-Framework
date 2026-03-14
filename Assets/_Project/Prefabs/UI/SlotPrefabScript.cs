using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour, IStorageSlot
{
    public int slotScriptIndex;
    public ItemInstance itemInstance;
    
    public ItemInstance Item => itemInstance;
    public int Index => slotScriptIndex;

    [Header("UI References")]
    [SerializeField] private Image itemIconDisplay;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemStackText;

    public void UpdateSlot(ItemInstance newItem)
    {
        itemInstance = newItem;
        // Refresh the UI state immediately when data changes
        SetVisibility(true);
    }

    public void SetVisibility(bool state)
    {
        bool hasItem = itemInstance != null && itemInstance.Data != null;
        
        // Final condition: Show only if requested AND an item exists
        bool showElements = state && hasItem;

        // Apply to Icon
        if (itemIconDisplay != null)
        {
            itemIconDisplay.sprite = hasItem ? itemInstance.Data.itemIcon : null;
            itemIconDisplay.enabled = showElements;
        }

        // Apply to Name
        if (itemNameText != null)
        {
            itemNameText.text = hasItem ? itemInstance.Data.itemName : "";
            itemNameText.enabled = showElements;
        }

        // Apply to Stack Count
        if (itemStackText != null)
        {
            bool shouldShowStack = showElements && itemInstance.Data.isStackable && itemInstance.stackSize > 1;
            itemStackText.text = shouldShowStack ? itemInstance.stackSize.ToString() : "";
            itemStackText.enabled = shouldShowStack;
        }
    }
}