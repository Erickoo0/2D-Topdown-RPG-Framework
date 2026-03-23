using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private SaveManager saveManager;
    
    public static bool IsGamePaused { get; private set; } = false;

    public static void SetPause(bool pause)
    {
        IsGamePaused = pause;  
    } 
    
    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool newState = !IsGamePaused;
            SetPause(newState);
            if (pauseMenu != null) pauseMenu.SetActive(newState);
        }
    }

    public void OnSaveButtonClicked() => saveManager.SaveGame();
    
    public void OnLoadButtonClicked() => saveManager.LoadGame();
    
    public void OnExitButtonClicked() => Application.Quit();
}
