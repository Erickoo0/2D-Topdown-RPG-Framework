using UnityEngine;

public interface IStorageSlot
{
    ItemData Item { get; }
    int Index { get; }
    void UpdateSlot(ItemData newItem);
}
