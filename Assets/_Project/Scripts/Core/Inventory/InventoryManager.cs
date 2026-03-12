using System;
using UnityEngine;

/// <summary>
/// Manages the global player inventory using a Singleton pattern. 
/// Handles item storage, addition, and swapping, while notifying listeners of slot changes via events.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    // Singleton Pattern: Accessible from any script
    public static InventoryManager Instance { get; private set; }

    // Observer Pattern: This event signals a slot has changed and displays the slot index number
    public event Action<int> OnSlotUpdated;

    [Header("Inventory Settings")] [SerializeField]
    private int inventorySize = 20;

    // An empty array of items (scriptable objects)
    public ItemData[] itemsList;

    private void Awake()
    {
        // Singleton Pattern: Safety measure to prevent new additional InventoryManagers from being created
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this; // Assign This ID to variable

        InitializeInventory();
    }

    /// <summary>
    /// Initializes an empty array to hold our items.
    /// We do it here in case we change inventorySize in a save file later
    /// </summary>
    private void InitializeInventory()
    {
        itemsList = new ItemData[inventorySize];
    }

    /// <summary>
    ///  Adding item to nearest empty slot in itemsList.
    /// </summary>
    public bool AddItems(ItemData item)
    {
        // Checks for empty slot in itemsList array
        for (int i = 0; i < itemsList.Length; i++)
        {
            if (itemsList[i] == null)
            {
                itemsList[i] = item; // Adds the item

                // Trigger Event
                OnSlotUpdated?.Invoke(i);
                return true;
            }
        }

        return false; // If itemsList is full
    }

    /// <summary>
    /// Swapping item between two itemsList indexes.
    /// </summary>
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
}
