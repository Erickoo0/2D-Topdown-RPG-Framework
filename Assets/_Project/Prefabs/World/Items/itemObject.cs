using UnityEngine;

/// <summary>
/// The Physical: The 2D model and Object of an Item in the game world
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class ItemObject : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private ItemData startingItemData; // Used ONLY when placing items manually via the editor

    private ItemInstance _itemInstance;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // If we assigned starting item in the inspector, then initialize it
        if (startingItemData != null) InitializeItem(new ItemInstance(startingItemData));
        
    }

    /// <summary>
    /// Updates the object with an existing instance (used when dropping items)
    /// </summary>
    public void InitializeItem(ItemInstance newItemInstance)
    {
        _itemInstance = newItemInstance;
        _spriteRenderer.sprite = _itemInstance.Data.itemIcon;
        gameObject.name = _itemInstance.Data.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _itemInstance != null)
        {
            bool wasPickedUp = InventoryManager.Instance.AddItems(_itemInstance);
            
            if (wasPickedUp)
            {
                // Add other effects here like sound later
                Destroy(gameObject);
            }
        }
    }
}
