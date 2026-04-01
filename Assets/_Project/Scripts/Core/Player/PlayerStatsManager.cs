using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }
    
    [SerializeField] private GameObject playerStatsPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.unityLogger.Log("Multiple PlayerStatsManagers detected. Disabling script.");
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        transform.position = GameObject.Find("Player").transform.position;
    }
    
    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!playerStatsPanel.activeSelf)EventBus.RequestOpenMenu(playerStatsPanel);
        else if (playerStatsPanel.activeSelf) EventBus.RequestCloseMenu(playerStatsPanel);
    }
}
