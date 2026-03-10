using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTest : MonoBehaviour
{
    public ItemData testItem;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            InventoryManager.Instance.AddItems(testItem);
        }
    }
    
}
