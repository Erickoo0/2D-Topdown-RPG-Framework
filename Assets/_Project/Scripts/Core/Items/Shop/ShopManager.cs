using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("References")] 
    [SerializeField] private GameObject shopMainPanel;
    [SerializeField] private GameObject shopItemPanel;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private InputAction menuKey;
    
    private void OnEnable() => menuKey.Enable();
    
    private void OnDisable() => menuKey.Disable();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple ShopManagers detected. Disabling script.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Event listeners
        EventBus.OnDialogueEventRequested += SetupShop;
    }

    private void Update()
    {
        if  (menuKey.WasPressedThisFrame())
        {
            if (shopMainPanel.activeSelf) shopMainPanel.SetActive(false);
        }
    }
    
    private void SetupShop(string dialogueEvent, object data)
    {
        if (dialogueEvent == "ShopOpen")
        {
            // Cast the object back to an array type
            ItemData[] shopList = data as ItemData[];
            
            // Safety Check
            if (shopList == null || shopList.Length == 0) return;
            
            shopMainPanel.SetActive(true);
            CreateShopItems(shopList);
        }
    }

    private void CreateShopItems(ItemData[] shopList)
    {
        foreach (ItemData shopItemData in shopList)
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
