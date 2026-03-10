using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent; // The Grid Layout Group
    
    // Keep a list of Slot scripts we created
    private List<Slot> uiSlots = new List<Slot>();

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
        // Create physical slots based on the InventoryManager.cs size
        for (int i = 0; i < InventoryManager.Instance.itemsList.Length; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            Slot slotScript = go.GetComponent<Slot>();

            slotScript.slotIndex = i; // Assign ID
            uiSlots.Add(slotScript);
        }
    }

    private void RefreshSlotUI(int index)
    {
        // Get data from InventoryManager.cs
        ItemData newData = InventoryManager.Instance.itemsList[index];
        
        // Tell the specific slot ui componenet to update its information
        uiSlots[index].UpdateSlot(newData);
    }
    
}