using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class ItemObject : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private ItemData startingItemData; // Used ONLY when placing items manually via the editor

    [Header("Spawn Animation")]
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private float bounceHeight = 0.65f;
    [SerializeField] private int bounceCount = 3;
    [SerializeField] private float flyDuration = 0.4f;
    [SerializeField] private float jumpPower = 1.2f;
    
    private ItemInstance _itemInstance;
    private SpriteRenderer _spriteRenderer;
    private bool _canBePickedUp = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // If we assigned starting item in the inspector, then initialize it
        if (startingItemData != null) InitializeItem(new ItemInstance(startingItemData));
        
    }

    public void InitializeItem(ItemInstance newItemInstance, Vector3? dropTarget = null)
    {
        _itemInstance = newItemInstance;
        _spriteRenderer.sprite = _itemInstance.Data.itemIcon;
        gameObject.name = _itemInstance.Data.itemName;

        PlaySpawnAnimation(dropTarget);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_canBePickedUp) return;
        TryPickup(collision);
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_canBePickedUp) return;
        TryPickup(collision);
    }

    private void PlaySpawnAnimation(Vector3? dropTarget)
    {
        _canBePickedUp = false;
        
        // Create a sequence jump -> bounce
        Sequence spawnSequence = DOTween.Sequence();
        
        // Final position is either the provided target, or its current position if spawned directly
        Vector3 finalPosition = dropTarget ?? transform.position;
        
        // 1. Fly to dropTarget if it has been provided by a source
        if (dropTarget.HasValue)
        {
            // Adds DOJump animation to the spawnSequence
            spawnSequence.Append(transform.DOJump(finalPosition, jumpPower, 1, flyDuration).SetEase(Ease.Linear));
        }
        
        // 2. Bounce Logic
        for (int i = 0; i < bounceCount; i++)
        {
            // Decrease the height and duration each bounce
            float currentBounceHeight = bounceHeight * (1f - (i * 0.4f));
            float currentDuration = bounceDuration * (1f - (i * 0.2f));
        
            // Adds DOMoveY (Up) to the spawnSequence
            spawnSequence.Append(transform.DOMoveY(finalPosition.y + currentBounceHeight, currentDuration / 2)
                .SetEase(Ease.OutQuad));
            // Adds DOMoveY (Down) to the spawnSequence
            spawnSequence.Append(transform.DOMoveY(finalPosition.y, currentDuration / 2)
                .SetEase(Ease.InQuad));
        }
        // 3. Enable Pickup logic ONLY after the entire sequence finishes
        spawnSequence.OnComplete(() => _canBePickedUp = true);
    }

    private void TryPickup(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _itemInstance != null)
        {
            _canBePickedUp = false; // Set to false since item is now picked up
            // Tell InventoryManager to add item to inventory
            bool wasPickedUp = InventoryManager.Instance.AddItems(_itemInstance);
            if (wasPickedUp)
            {
                // Add other effects here like sound later
                Destroy(gameObject);
            }
        }
    }
}
