using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The Map: Connects ItemData.itemID to the ItemData in a Dictionary
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/Database")]
public class ItemDatabase : ScriptableObject
{
    [Tooltip("Add every ItemData in the game here")]
    public List<ItemData> allItems;
    
    // Key: Items String ID
    // Value: Items ItemData
    private Dictionary<string, ItemData> _itemDictionary;

    /// <summary>
    /// Converts ItemsLIst into an ItemsDictionary
    /// Called from InventoryManager
    /// </summary>
    public void Initialize()
    {
        // Create a fresh dictionary
        _itemDictionary = new Dictionary<string, ItemData>();

        // Loop through every item in the database
        foreach (ItemData itemData in allItems)
        {
            // Pairs itemID to itemData, returns warning if itemID has already been previously used
            if (!_itemDictionary.TryAdd(itemData.itemID, itemData))
            {
                Debug.LogWarning($"[ItemDatabase] Duplicate Item ID found: {itemData.itemID}. IDs must be unique!");
            }
        }
    }

    public ItemData GetItem(string itemID)
    {
        // Safety Check: If the dictionary hasnt been built yet, build it now
        if (_itemDictionary == null) Initialize();
        
        // If the ID exists, returns the Item, else return null
        // ReSharper disable once PossibleNullReferenceException
        return _itemDictionary.GetValueOrDefault(itemID);
    }
}
