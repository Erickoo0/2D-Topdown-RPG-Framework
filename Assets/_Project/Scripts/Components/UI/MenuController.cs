using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    
    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        EventBus.RequestOpenMenu(menuPanel);
        Debug.Log("Menu Opened");
    }
}
