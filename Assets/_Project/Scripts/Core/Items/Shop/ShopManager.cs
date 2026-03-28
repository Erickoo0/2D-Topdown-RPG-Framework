using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
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

    private void SetupShop(string dialogueEvent)
    {
        Debug.unityLogger.Log($"Setting up shop for action {dialogueEvent}");
    }
}
