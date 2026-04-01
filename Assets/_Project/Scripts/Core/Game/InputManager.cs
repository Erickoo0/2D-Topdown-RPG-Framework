using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        // Destroy duplicates if they accidentally get created
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the static instance to this specific object
        Instance = this;
        
        // Grab the component
        PlayerInput = GetComponent<PlayerInput>();
        
        // Keep this manager alive when changing scenes
        DontDestroyOnLoad(gameObject);
    }
    
}

