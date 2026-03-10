using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotIndex; // Unique Id of slot (Set within the InventoryManager.cs)
    public ItemData itemData; // Local reference to item
    
    [Header("Sprite References")]
    [SerializeField] private Image itemIconDisplay;

    private void Awake()
    {
        // Ensure we start clean
        UpdateSlot(null);
    }
    

    public void UpdateSlot(ItemData newItem)
    {
        itemData = newItem;

        if (itemData != null)
        {
            itemIconDisplay.sprite = itemData.itemIcon;
            itemIconDisplay.enabled = true;
        }
        else
        {
            itemIconDisplay.sprite = null;
            itemIconDisplay.enabled = false;
        }
        
    }
}
