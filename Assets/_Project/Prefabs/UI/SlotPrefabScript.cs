using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IStorageSlot
{
    public int slotScriptIndex;
    public ItemData itemData;
    
    public ItemData Item => itemData;
    public int Index => slotScriptIndex;

    [SerializeField] private Image itemIconDisplay;

    public void UpdateSlot(ItemData newItem)
    {
        itemData = newItem;
        bool hasItem = itemData != null;
        itemIconDisplay.sprite = hasItem ? itemData.itemIcon : null;
        itemIconDisplay.enabled = hasItem;
    }
}
