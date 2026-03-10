using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    [TextArea] public string itemDescription;
}
