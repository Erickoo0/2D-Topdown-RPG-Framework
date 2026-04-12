using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IInteractable
{
    private Health _health;
    
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public Sprite containerOpenedSprite;
    
    [Header("Chest Contents")]
    // The list of items to drop
    public List<ItemDrop> containerContents = new List<ItemDrop>(); 
    
    [Header("Drop Settings")]
    [SerializeField] private float spreadRadius = 1.5f;
    
    [System.Serializable]
    public struct ItemDrop
    {
        public ItemDataSo itemDataSo;
        public int dropAmount;
    }
    
    public void Awake()
    {
        if (string.IsNullOrEmpty(ChestID)) GlobalHelper.GenerateUniqueID(gameObject);
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDeath += OpenContainer;
        }
    }
    
    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDeath -= OpenContainer;
        }
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }
    
    public void Interact()
    {
        if (!CanInteract()) return;
        OpenContainer();
    }

    public void OpenContainer()
    {
        // If the chest is already opened, do nothing
        if (IsOpened) return;
        SetOpened(true);
        
        if (containerContents == null || containerContents.Count == 0) return;

        // 1. Count how many physical objects to spawn
        int itemsToDrop = 0;
        foreach (ItemDrop itemDrop in containerContents)
        {
            if (itemDrop.itemDataSo != null) itemsToDrop++;
        }
        
        // 2. Calculate angle between each item
        float angleStep = 360f / itemsToDrop;
        float currentAngle = UnityEngine.Random.Range(0f, 360f);
        
        foreach (ItemDrop itemDrop in containerContents)
        {
            if (itemDrop.itemDataSo == null)
            {
                Debug.LogWarning("Chest item is null!");
                continue;
            }
            
            // 3. Calculate the target position for each item
            Vector3 spawnPos = transform.position;
            Vector3 targetPos = CalculateDropPosition(currentAngle);
            
            // 2. Spawn the object at the spawnPos
            GameObject droppedItemObj = Instantiate(itemDrop.itemDataSo.ItemObject, spawnPos, Quaternion.identity);
            
            // 3. Check if the droppedItemObj has ItemObject component, if so, initialize it
            if (droppedItemObj.TryGetComponent(out ItemObject itemObject))
            {
                ItemInstance itemInstance = new ItemInstance(itemDrop.itemDataSo, itemDrop.dropAmount); // Create an item instance
                itemObject.SetItemObject(itemInstance, targetPos);
            }
            
            // 4. Inccrement the angle for the next item
            currentAngle += angleStep;
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            if (TryGetComponent(out SpriteRenderer sr))
            {
                sr.sprite = containerOpenedSprite;
            }
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
        Vector2 finalOffset = direction * distance;
        
        // 4. Return the final position
        return transform.position + new Vector3(finalOffset.x, finalOffset.y - 0.5f, 0);
    }
}