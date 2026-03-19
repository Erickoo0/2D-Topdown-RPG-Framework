using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The Blueprint: Static Item Data
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item ID")] 
    [Tooltip("This must be unique for every item")]
    public string itemID;
    
    [Header("Item Data")]
    public Sprite itemIcon;
    public string itemName;
    [TextArea] public string itemDescription;
    public GameObject itemObject; // The physical item that gets picked up

    [Header("Item Properties")] 
    public bool isStackable;
    public int maxStackSize = 60;
    
    public bool isUsable;

    public virtual bool Use(GameObject user, ItemInstance itemInstance)
    {
        return false;
    }
}
