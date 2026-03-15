using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the global player inventory using a Singleton pattern. 
/// Handles item storage, addition, and swapping, while notifying listeners of slot changes via events.
/// </summary>
public class InventoryManager : MonoBehaviour, ISaveable
{
    public static InventoryManager Instance { get; private set; }
    
    public event Action<int> OnSlotUpdated;
    public event Action<ItemInstance> OnItemAddedToInventory;
    public event Action<int> OnActiveSlotIndexChanged;
    
    [Header("Item Database")] 
    public ItemDatabase itemDatabase; // Drag our database here

    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20;
    
    // An empty array of items 
    public ItemInstance[] itemsList;

    private void Awake()
    {
        // Singleton Pattern: Safety measure to prevent new additional InventoryManagers from being created
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple InventoryManagers detected. Disabling script.");
            Destroy(gameObject);
            return;
        }
        Instance = this; // Assign This ID to variable
        
        if (itemDatabase != null) itemDatabase.Initialize();

        InitializeInventory();
    }
    
    private void InitializeInventory()
    {
        itemsList = new ItemInstance[inventorySize];
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public bool AddItems(ItemInstance item)
    {
        // Try to stack first if the item is stackable
        if (item.Data.isStackable == true)
        { 
            for (int i = 0; i < itemsList.Length; i++)
            {
                // Skip empty slots and slots with different items
                if (itemsList[i] == null || itemsList[i].Data != item.Data) continue;
                
                int spaceLeft = itemsList[i].Data.maxStackSize - itemsList[i].stackSize;
                
                // Skip full slots
                if (spaceLeft <= 0) continue;

                // If whole new stack fits in current slot
                if (item.stackSize <= spaceLeft)
                {
                    itemsList[i].stackSize += item.stackSize;
                    OnSlotUpdated?.Invoke(i);
                    OnItemAddedToInventory?.Invoke(item);
                    return true;
                }
                // If partial new stack fits in current slow
                else
                {
                    itemsList[i].stackSize = itemsList[i].Data.maxStackSize;
                    item.stackSize -= spaceLeft;
                    OnSlotUpdated?.Invoke(i);
                    // Do not return true yet, code continues to find open slot for remainder
                }
            }
        }
        
        // If not stackable / No free stack available
        for (int i = 0; i < itemsList.Length; i++)
        {
            if (itemsList[i] == null)
            {
                itemsList[i] = item; // Adds the item
                OnSlotUpdated?.Invoke(i);
                OnItemAddedToInventory?.Invoke(item);
                return true;
            }
        }

        Debug.unityLogger.Log("Iventory is full");
        return false; // If inventory is full
    }

    public void RemoveItems(int index)
    {
        if (index < 0 || index >= itemsList.Length) return;
        ItemInstance item = itemsList[index];
        item.stackSize--;
        if (item.stackSize <= 0) itemsList[index] = null;

        
        OnSlotUpdated?.Invoke(index);
    }
    
    public void SwapItems(int indexA, int indexB)
    {
        // Check if indexes are out of range of itemsList array
        if (indexA < 0 || indexA >= itemsList.Length || indexB < 0 || indexB >= itemsList.Length) return;

        // Swaps item from A and B using modern C# deconstruction
        (itemsList[indexA], itemsList[indexB]) = (itemsList[indexB], itemsList[indexA]);

        // Trigger event for BOTH slots involved in the swap
        OnSlotUpdated?.Invoke(indexA);
        OnSlotUpdated?.Invoke(indexB);
    }

    public void DropItems(int index, Vector3 spawnPosition)
    {
        // Safety Check: Make sure the slot is not already empty
        if (itemsList[index] == null) return;
        
        // Spawn the item
        GameObject droppedItem = Instantiate(itemsList[index].Data.itemObject, spawnPosition,  Quaternion.identity );
        // If droppedItem has compoent ItemObject, then attach it to the variable itemObject, else pass
        if (droppedItem.TryGetComponent(out ItemObject itemObject))
        {
            itemObject.InitializeItem(itemsList[index]);
        }
        
        // Clear the slot and notify the UI
        itemsList[index] = null;
        OnSlotUpdated?.Invoke(index);
    }

    public void PopulateSaveData(SaveData saveData)
    {
        // Create a list of SavedSlots
        List<SavedSlot> savedSlots = new List<SavedSlot>();
        
        // Loops through inventory and adds any non-empty slots into the savedSlots array
        for (int i = 0; i < itemsList.Length; i++)
        {
            if (itemsList[i] != null && itemsList[i].Data != null)
            {
                // Create SavedSlots passing slot index #, itemID, and stackSize
                savedSlots.Add(new SavedSlot
                {
                    index = i,
                    itemID = itemsList[i].Data.itemID,
                    itemStackSize = itemsList[i].stackSize
                });
            }
        }
        
        // Send the SavedSLots list to SaveData
        saveData.savedSlotList = savedSlots;
    }
    
    public void LoadFromSaveData(SaveData saveData)
    {
        // Clear current inventory
        InitializeInventory(); 

        // Rebuild instances from the data inside the "box"
        foreach (SavedSlot savedSlot in saveData.savedSlotList)
        {
            // Safety check: ensure index is within bounds
            if (savedSlot.index < 0 || savedSlot.index >= itemsList.Length) continue;
        
            // Convert the itemIDs from savedSlot into ItemData
            ItemData itemData = itemDatabase.GetItem(savedSlot.itemID);
        
            if (itemData == null)
            {
                Debug.LogWarning($"[Inventory] Could not find item with ID: {savedSlot.itemID} in database.");
                continue;
            }
            
            // Use the ItemData to add ItemInstances to the itemsList
            itemsList[savedSlot.index] = new ItemInstance(itemData, savedSlot.itemStackSize);
        }

        // 3. Notify the UI to refresh ALL slots
        for (int i = 0; i < itemsList.Length; i++)
        {
            OnSlotUpdated?.Invoke(i);
        }
    }

    public void ChangeActiveSlot(int index)
    {
        ItemInstance selectedItem = itemsList[index];
        
        OnActiveSlotIndexChanged?.Invoke(index);
    }

}
