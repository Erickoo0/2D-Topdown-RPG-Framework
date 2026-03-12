using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A test script that adds items to inventory from a pool of <see cred="ItemData"/>
/// </summary>
public class InventoryTest : MonoBehaviour
{
    [Header("Item Pool")]
    public ItemData[] testItemPool;

    void Update()
    {
        if (testItemPool == null || testItemPool.Length == 0) return;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            
           ItemData selectedItem = SelectRandomItem();
            
           bool ifItemAdded = InventoryManager.Instance.AddItems(selectedItem);
           if (ifItemAdded) Debug.unityLogger.Log($"Item {selectedItem.itemName} added to inventory");
           else Debug.unityLogger.Log($"Inventory is full! Cannot add more items");
        }
    }

    private ItemData SelectRandomItem()
    {
        int randomIndex = Random.Range(0, testItemPool.Length);
        ItemData selectedItem = testItemPool[randomIndex];
        return selectedItem;
    }

    
}
