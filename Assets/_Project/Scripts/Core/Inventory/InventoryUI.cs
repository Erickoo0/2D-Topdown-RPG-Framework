using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the visual representation of the player's inventory. 
/// Spawns <see cref="Slot"/> elements and listens for data changes to refresh specific slots.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab; // The Slots to spawn
    public Transform slotParent; // The Grid Layout Group
    
    // Keep a list of Slot scripts we created
    private List<Slot> slotScriptsList = new List<Slot>();

    private void Start()
    {
        SetupUI();
        
        // Subscribe to InventoryManager.cs event
        InventoryManager.Instance.OnSlotUpdated += RefreshSlotUI;
        
        // Initial Refresh: Sync UI with whatever data is already in the Inventory Manager (Saved data)
        for (int i = 0; i < InventoryManager.Instance.itemsList.Length; i++)
        {
            RefreshSlotUI(i);
        }
    }

    private void OnDestroy()
    {
        // Ubsubscribe when destroyed to prevent memory leaks
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSlotUpdated -= RefreshSlotUI;
        }
    }

    private void SetupUI()
    {
        
        for (int i = 0; i < InventoryManager.Instance.itemsList.Length; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent); // Creates Slot Prefab
            Slot slotScript = slot.GetComponent<Slot>(); // Find the Slot Prefabs Slot Script

            slotScript.slotScriptIndex = i; // Assign each slot an index number from the for loop
            slotScriptsList.Add(slotScript); // Add the slotscript to a list
        }
    }

    private void RefreshSlotUI(int index)
    {
        // Get items from item list in InventoryManager.cs
        ItemData newData = InventoryManager.Instance.itemsList[index];
        
        // Tells the slot script to update its data
        slotScriptsList[index].UpdateSlot(newData);
    }
    
}