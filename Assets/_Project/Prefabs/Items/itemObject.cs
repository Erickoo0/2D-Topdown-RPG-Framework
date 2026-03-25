using System.Threading.Tasks;
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

    [Header("Pickup Animation")]
    [SerializeField] private float pullSpeed = 0.2f;
    [SerializeField] private Ease pullEase = Ease.InBack; // Snappy vacuum feel
    
    private ItemInstance _itemInstance;
    private SpriteRenderer _spriteRenderer;
    private bool _canBePickedUp = false;
    private Sequence _activeSequence; // Spawn Animation sequence

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // If we assigned starting item in the inspector, then initialize it
        if (startingItemData != null) SetItemObject(new ItemInstance(startingItemData));
        
    }

    private void OnDestroy() => _activeSequence?.Kill(); // Kill the animation sequence on destroy

    private void Update()
    {
        if (_itemInstance?.Data != null && _itemInstance.Data.animated)
        {
            _spriteRenderer.sprite = GlobalHelper.GetAnimatedSprite(_itemInstance.Data);
        }
    }
    
    public void SetItemObject(ItemInstance newItemInstance, Vector3? dropTarget = null, bool animate = true)
    {
        _itemInstance = newItemInstance;
        gameObject.name = _itemInstance.Data.itemName;

        // Set initial sprite
        if (newItemInstance.Data != null)
            _spriteRenderer.sprite = _itemInstance.Data.animated ? GlobalHelper.GetAnimatedSprite(_itemInstance.Data) : _itemInstance.Data.itemIconAnimated[0];
        
        if (!animate) return; // Skip the animation
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
        _activeSequence = DOTween.Sequence();
        
        // Final position is either the provided target, or its current position if spawned directly
        Vector3 finalPosition = dropTarget ?? transform.position;
        
        // 1. Fly to dropTarget if it has been provided by a source
        if (dropTarget.HasValue)
        {
            // Adds DOJump animation to the spawnSequence
            _activeSequence.Append(transform.DOJump(finalPosition, jumpPower, 1, flyDuration).SetEase(Ease.Linear));
            
            // 2. Enable Pickup AFTER fly animation
            _activeSequence.AppendCallback(() => _canBePickedUp = true);
        }
        
        // 3. Bounce Logic
        for (int i = 0; i < bounceCount; i++)
        {
            // Decrease the height and duration each bounce
            float currentBounceHeight = bounceHeight * (1f - (i * 0.4f));
            float currentDuration = bounceDuration * (1f - (i * 0.2f));
        
            // Adds DOMoveY (Up) to the spawnSequence
            _activeSequence.Append(transform.DOMoveY(finalPosition.y + currentBounceHeight, currentDuration / 2)
                .SetEase(Ease.OutQuad));
            // Adds DOMoveY (Down) to the spawnSequence
            _activeSequence.Append(transform.DOMoveY(finalPosition.y, currentDuration / 2)
                .SetEase(Ease.InQuad));
        }
        
        // 4. Clean up the sequence reference 
        _activeSequence.OnComplete(() =>
        {
            _activeSequence = null;
            _canBePickedUp = true; // Enable pickup after the animation is finished
        });
    }

    private async void TryPickup(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || _itemInstance == null) return;
        
        // 1. Lock out further triggers
        _canBePickedUp = false; // Set to false since item is now picked up

        // 2. Kill the current animation sequence 
        _activeSequence.Kill();

        // 3. Play the vacuum animation towards the player, pause the code here untill animation finishes.
        await transform.DOMove(collision.transform.position, pullSpeed).SetEase(pullEase).AsyncWaitForCompletion();
            
        // 4. Execute pickup
        bool wasPickedUp = InventoryManager.Instance.AddItems(_itemInstance);
        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }
}
