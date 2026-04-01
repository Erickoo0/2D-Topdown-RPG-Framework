using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private SaveManager saveManager;
    
    public static bool IsGamePaused { get; private set; } = false;

    public static void SetPause(bool pause) => IsGamePaused = pause;  
    
    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        EventBus.RequestOpenMenu( pauseMenuPanel);
        Debug.Log("Pause Menu Opened");
    }

    public void OnSaveButtonClicked() => saveManager.SaveGame();
    
    public void OnLoadButtonClicked() => saveManager.LoadGame();
    
    public void OnExitButtonClicked() => Application.Quit();
}
