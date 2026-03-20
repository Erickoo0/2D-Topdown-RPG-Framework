using System;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public Sprite chestOpenedSprite;
    
    [Header("Chest Contents")]
    public ItemData itemData; // The Data of the item that will be dropped when the chest is opened
    public int dropAmount = 1;

    public void Start()
    {
        if (ChestID == null) GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }
    
    public void Interact()
    {
        if (!CanInteract()) return;
        
        // Open Chest
        OpenChest();
    }

    public void OpenChest()
    {
        // Set Opened
        SetOpened(true);
        
        // Drop Item Logic
        if (itemData == null) return;
        // Create an instance from the ItemData
        ItemInstance instance = new ItemInstance(itemData, dropAmount); // 1 item by default
        
        // Spawn the physical object
        GameObject droppedItemObj = Instantiate(itemData.itemObject, transform.position + Vector3.down, Quaternion.identity);
        
        // Initialize it with the instance
        if (droppedItemObj.TryGetComponent(out ItemObject itemObject))
        {
            itemObject.InitializeItem(instance);
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = chestOpenedSprite;
        }
        
    }
}
