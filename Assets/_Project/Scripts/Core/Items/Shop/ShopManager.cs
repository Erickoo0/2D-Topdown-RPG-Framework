using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{

    [Header("References")] 
    [SerializeField] private GameObject shopMainPanel;
    [SerializeField] private GameObject shopItemPanel;
    [SerializeField] private GameObject shopItemPrefab;
    
    private void Awake()
    { 
        EventBus.OnDialogueEventRequested += SetupShop;
    }
    
    private void SetupShop(string dialogueEvent, object data)
    {
        if (dialogueEvent != "ShopOpen") return;
        
        // Cast the object back to an array type
        ItemDataSo[] shopList = data as ItemDataSo[];
        
        // Safety Check
        if (shopList == null || shopList.Length == 0) return;
        
        CreateShopItems(shopList);
        
        // Send request to UIManager to handle the showing of UI
        EventBus.RequestOpenMenu(shopMainPanel);
        
    }

    private void CreateShopItems(ItemDataSo[] shopList)
    {
        // Destroy old items
        foreach (Transform child in shopItemPanel.transform) Destroy(child.gameObject);
        
        foreach (ItemDataSo shopItemData in shopList)
        {
            // Create the button and get the components
            GameObject shopItem = Instantiate(shopItemPrefab, shopItemPanel.transform);
            var iconComponent = shopItem.transform.Find("Item Icon").GetComponent<Image>();
            var nameComponent = shopItem.transform.Find("Item Name").GetComponent<TextMeshProUGUI>();
            var buttonComponent = shopItem.GetComponent<Button>();

            // Update the data
            iconComponent.sprite = shopItemData.ItemIcon[0];
            nameComponent.text = shopItemData.ItemName;
            
            // Add click event listener to button
            buttonComponent.onClick.AddListener(() =>
            {
                ItemInstance newPurchase = new ItemInstance(shopItemData, 1);
                InventoryManager.Instance.AddItems(newPurchase);
            });
        }
    }
}
