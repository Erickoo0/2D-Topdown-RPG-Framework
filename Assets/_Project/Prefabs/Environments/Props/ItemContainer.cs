using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemContainer : MonoBehaviour, IInteractable
{
    [Header("References")] 
    [SerializeField] private Sprite containerOpenedSprite;
    private SpriteRenderer _spriteRenderer;
    private Health _health;
    
    [Header("Drop Settings")]
    [SerializeField] private List<ItemDrop> containerContents = new List<ItemDrop>();
    [SerializeField] private float spreadRadius = 1.5f;
    
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    
    [System.Serializable]
    public struct ItemDrop
    {
        public ItemDataSo itemDataSo;
        public int dropAmount;
    }
    
    public void Awake()
    {
        _health = GetComponent<Health>();
        _spriteRenderer =  GetComponent<SpriteRenderer>();
        
        if (string.IsNullOrEmpty(ChestID)) 
            GlobalHelper.GenerateUniqueID(gameObject);
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDeath += OpenContainer;
    }
    
    private void OnDisable()
    {
        if (_health != null) 
            _health.OnDeath -= OpenContainer;
    }

    public bool CanInteract() => !IsOpened;
    
    public void Interact()
    {
        if (CanInteract())
            OpenContainer();
    }

    public void OpenContainer()
    {
        if (IsOpened) return;

        UpdateVisuals();
        DropItems();
    }

    private void UpdateVisuals()
    {
        IsOpened = true;
        if (_spriteRenderer != null && containerOpenedSprite != null)
            _spriteRenderer.sprite = containerOpenedSprite;
    }

    private void DropItems()
    {
        // Filter out null data and add valid data to list
        var validDrops = containerContents.Where(d => d.itemDataSo != null).ToList();
        if (validDrops.Count <= 0) return;
        
        float angleStep = 360f / validDrops.Count;
        float currentAngle = Random.Range(0f, 360f);

        foreach (ItemDrop drop in validDrops)
        {
            SpawnItem(drop, currentAngle);
            currentAngle += angleStep;
        }
    }

    private void SpawnItem(ItemDrop drop, float angle)
    {
        Vector3 targetPosition = CalculateDropPosition(angle);
        GameObject droppedItem =  Instantiate(drop.itemDataSo.ItemObject, transform.position, Quaternion.identity);

        if (droppedItem.TryGetComponent(out ItemObject itemObject))
        {
            var instance = new ItemInstance(drop.itemDataSo, drop.dropAmount);
            itemObject.SetItemObject(instance, targetPosition);
        }
    }

    public Vector3 CalculateDropPosition(float angleDegrees)
    {
        // 1. Convert angle from degrees to radian
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        // 2. Get the normalized x and y direction
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));    
        // 3. Pick a random distance to keep the spread feeling organic
        float distance = UnityEngine.Random.Range(0.5f, spreadRadius);
        
        // 4. Return the final position
        // Offset Y slightly to look better in 2D top-down perspective
        return transform.position + new Vector3(direction.x * distance, (direction.y * distance) - 0.5f, 0);    }
}