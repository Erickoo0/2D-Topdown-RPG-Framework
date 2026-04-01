using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    private Stack<GameObject> _panelStack = new Stack<GameObject>();
    
    private void OnEnable()
    {
        EventBus.OnMenuOpenRequested += HandleMenuOpenRequest;
        EventBus.OnMenuCloseRequested += HandleMenuCloseRequest;
    }
    
    private void OnDisable()
    {
        EventBus.OnMenuOpenRequested -= HandleMenuOpenRequest;
        EventBus.OnMenuCloseRequested -= HandleMenuCloseRequest;
    }

    private void HandleMenuOpenRequest(GameObject menu)
    {
        // 1. If first menu is opening, switch to UI Input Map
        if (_panelStack.Count == 0)
        {
            InputManager.Instance.PlayerInput.SwitchCurrentActionMap("UI");
            
            PauseManager.SetPause(true);
        }
        
        // 2. Set Menu and add to stack
        menu.SetActive(true);
        _panelStack.Push(menu);
    }

    // When the ESC key gets pressed, send a close request
    public void OnCloseMenuInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // Guard Clause to stop the input from firing 3 times

        if (_panelStack.Count > 0) HandleMenuCloseRequest();
    }

    private void HandleMenuCloseRequest(GameObject menu = null)
    {
        if (_panelStack.Count <= 0) return;
        
        // 1. Remove top menu if no specific menu is requested
        if (menu == null)
        {
            GameObject topMenu = _panelStack.Pop();
            topMenu.SetActive(false);
        }
        else // 2. Remove specific menu
        {
            // Check the top of the stack, if it is the specific menu, simply remove it
            if (_panelStack.Peek() == menu) _panelStack.Pop();
            else // Otherwise, rebuild the stack without that specific menu (cant remove item from middle of stack)
            {
                // Create a list of the current stack
                List<GameObject> tempStack = new List<GameObject>(_panelStack);
                tempStack.Remove(menu); // Remove the menu from the stack
                _panelStack = new Stack<GameObject>(tempStack); // Rebuild the stack
            }
            menu.SetActive(false);
        }
        
        // 3. If no menus left, return to gameplay
        if (_panelStack.Count <= 0)
        {
            InputManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
            PauseManager.SetPause(false);
        }
    }
}