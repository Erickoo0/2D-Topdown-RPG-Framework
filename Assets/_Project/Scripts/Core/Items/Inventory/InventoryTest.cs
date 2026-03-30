using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTest : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Bind the key or button you want to use to spawn items.")]
    public InputAction addItemHotkey;
    
    private GameObject _player;

    private void OnEnable() => addItemHotkey.Enable();
    private void OnDisable() => addItemHotkey.Disable();
    
    private void Start() => _player = GameObject.FindGameObjectWithTag("Player");
    
    void Update()
    {
        // 1. Wait for hotkey press
        if (!addItemHotkey.WasPressedThisFrame()) return;
        
        // 2. Grab the database directly Inventory Manager
        ItemDatabase db = InventoryManager.Instance.itemDatabase;
        if (db == null || db.allItems.Count == 0) return;

        // 3. Pick a random item and create the data
        ItemData randomItemData = db.allItems[Random.Range(0, db.allItems.Count)];
        int randomAmount = randomItemData.IsStackable ? Random.Range(1, 6) : 1;
        ItemInstance newItemInstance = new ItemInstance(randomItemData, randomAmount);
        
        // 4. Spawn and Initialize the item
        GameObject droppedItemObj = Instantiate(newItemInstance.Data.ItemObject, _player.transform.position, Quaternion.identity);
        if (droppedItemObj.TryGetComponent(out ItemObject itemObject))
        {
            itemObject.SetItemObject(newItemInstance);
            Debug.unityLogger.Log($"Spawned {randomAmount}x {randomItemData.ItemName} in the world!");
        }

    }
}