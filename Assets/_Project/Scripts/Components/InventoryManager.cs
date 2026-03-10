using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton Instance: Accessible from any script
    public static InventoryManager Instance { get; private set; }

    // Observer Pattern: This event signals a slot has changed and displays the slot index number
    public event Action<int> OnSlotUpdated;

    [Header("Inventory Settings")] [SerializeField]
    private int inventorySize = 20;

    // An array of items (scriptable objects)
    public ItemData[] itemsList;

    private void Awake()
    {
        // Singleton Pattern Implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeInventory();
    }

    private void InitializeInventory()
    {
        itemsList = new ItemData[inventorySize];
    }

    /// <summary>
    ///  Adding item to nearest empty slot in itemsList, and signaling event.
    /// </summary>
    public bool AddItems(ItemData item)
    {
        // Checks for empty slot in itemsList
        for (int i = 0; i < itemsList.Length; i++)
        {
            if (itemsList[i] == null)
            {
                itemsList[i] = item;

                // Trigger Event
                OnSlotUpdated?.Invoke(i);
                return true;
            }
        }

        return false; // If itemsList is full
    }

    /// <summary>
    /// Swapping data between two slots, and signaling event.
    /// </summary>
    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= itemsList.Length || indexB < 0 || indexB >= itemsList.Length) return;

        ItemData temp = itemsList[indexA];
        itemsList[indexA] = itemsList[indexB];
        itemsList[indexB] = temp;

        // Trigger event for BOTH slots involved in the swap
        OnSlotUpdated?.Invoke(indexA);
        OnSlotUpdated?.Invoke(indexB);
    }
}
