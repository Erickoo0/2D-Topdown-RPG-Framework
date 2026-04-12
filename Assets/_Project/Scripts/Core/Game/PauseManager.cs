using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    
    [SerializeField] private GameObject pauseMenuPanel;
    
    public GameObject PauseMenuPanel => pauseMenuPanel;
    public static bool IsGamePaused { get; private set; } = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple Pause Managers detected. Disabling script.");
            Destroy(gameObject);
            return;
        }
        Instance = this; // Assign This ID to variable

    }
    
    public static void SetPause(bool pause) => IsGamePaused = pause;  
    
    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!pauseMenuPanel.activeSelf)EventBus.RequestOpenMenu(pauseMenuPanel);
        else if (pauseMenuPanel.activeSelf) EventBus.RequestCloseMenu(pauseMenuPanel);
    }

    public void OnSaveButtonClicked() => SaveManager.Instance.SaveGame();
    
    public void OnLoadButtonClicked() => SaveManager.Instance.LoadGame();
    
    public void OnExitButtonClicked()
    {
        // This part only runs inside the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // This part runs in the actual built game
        Application.Quit();
    }
}
