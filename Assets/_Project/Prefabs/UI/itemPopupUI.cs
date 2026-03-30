using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ItemPopup : MonoBehaviour
{
    public ItemInstance itemInstance;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemStackSize;

    [SerializeField] private float lifetime = 3f;

    private void Update()
    {
        if (itemInstance?.Data != null && itemInstance.Data.animated)
            itemIcon.sprite = GlobalHelper.GetAnimatedSprite(itemInstance.Data);
    }
    
    public void SetPopUp(ItemInstance newItem)
    {
        itemInstance = newItem;
        // Set the text
        itemName.text = itemInstance.Data.itemName;
        itemStackSize.text = itemInstance.stackSize.ToString();
        
        if (!itemInstance.Data.animated)
            itemIcon.sprite = itemInstance.Data.itemIcon[0];
        
        Destroy(gameObject, lifetime);
        
    }
}
