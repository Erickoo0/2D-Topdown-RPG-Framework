using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private InputAction menuKey;

    private void OnEnable() => menuKey.Enable();
    
    private void OnDisable() => menuKey.Disable();
    
    void Start() => menuCanvas.SetActive(false);
    
    void Update()
    {
        if (menuKey.WasPressedThisFrame())
        {
            // If game is paused from other sources, disable opening the menu
            if (!menuCanvas.activeSelf && PauseManager.IsGamePaused) return;
            // Inverses whatever the current menuCanvas state is. True -> False || False -> True
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PauseManager.SetPause(menuCanvas.activeSelf);
        }
    }
}
