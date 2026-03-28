using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    
    private void SetupShop(string dialogueEvent)
    {
        if (dialogueEvent == "ShopOpen")
        {
            shopMainPanel.SetActive(true);
        }
    }
}
