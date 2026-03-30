using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlotUI : MonoBehaviour, IStorageSlot
{
    //public int slotScriptIndex;
    [field: SerializeField] public int Index { get; private set; }
    public ItemInstance itemInstance => InventoryManager.Instance.itemsList[slotScriptIndex];

    [Header("UI References")]
    [SerializeField] private Image itemIconDisplay;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemStackText;

    private bool _isBeingDragged = false;

    private bool ShouldAnimate => itemInstance?.Data != null && itemInstance.Data.IsAnimated && !_isBeingDragged;

    private void Update()
    {
        if (ShouldAnimate)
            itemIconDisplay.sprite = GlobalHelper.GetAnimatedSprite(itemInstance.Data);
        
    }
    
    public void RefreshSlotUI()
    {
        var item = itemInstance; // Gets data from Inventory Manager via Property
        bool hasItem = item != null && item.Data != null; // Check if the slot has an item
        bool shouldShow = hasItem && !_isBeingDragged; // Hides elements while being dragged

        // Set the text
        itemNameText.text = hasItem ? item.Data.ItemName : null;
        itemStackText.text = hasItem ? item.stackSize.ToString() : null;
        
        // If not animated, set the sprite
        if (hasItem && itemInstance.Data.ItemIcon.Length == 1)
            itemIconDisplay.sprite = item.Data.ItemIcon[0];
        
        
        itemIconDisplay.enabled = shouldShow;
    }

    public void SetDraggingState(bool isDragging)
    {
        _isBeingDragged = isDragging;
        RefreshSlotUI();
    }
}