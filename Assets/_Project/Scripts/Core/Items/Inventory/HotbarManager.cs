using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }
    
    private void Awake()
    {       
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple HotbarManagers detected. Disabling script.");
            Destroy(gameObject);
            return;
        }
        Instance = this; // Assign This ID to variable
    }
    
    public void OnSelectSlot(InputAction.CallbackContext context)
    {
        // Detects key pressed down input
        if (context.ReadValueAsButton())
        {
            // Gets the name of the key pressed
            string keyName = context.control.name; 
            // Gets the last char of the keyName (e.g. keyboard1 -> 1)
            char lastChar = keyName[keyName.Length - 1];

            // Safety Check: Key is a number
            if (char.IsDigit(lastChar))
            {
                // Subtract 0 from any number gives you its actual integer value
                int val = lastChar - '0';
                // Converts 0 - > 10
                if (val == 0) val = 10; 
                // Converts Key pressed to Slot Index
                int targetIndex = val - 1;
                
                // Checks if targetIndex is in range of hotbar
                if (targetIndex >= 0 && targetIndex < 9)
                {
                    InventoryManager.Instance.ChangeActiveSlot(targetIndex);
                }
            }
        }
    }
    
}