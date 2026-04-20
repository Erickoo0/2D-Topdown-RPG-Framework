using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombatController : MonoBehaviour
{
    [Header("Attack Library")]
    [SerializeField] private List<AttackData> attackLibrary;
    
    private PlayerController _playerController;
    private Camera _mainCam;
    private Vector2 _rawMousePosition;
    public CombatContext CombatContext { get; private set; } = new CombatContext();

    private void Awake()
    {
        _mainCam = Camera.main;
        _playerController = GetComponent<PlayerController>();
    }
    
    
    //Tracks Mouse Movement
    public void OnPoint(InputAction.CallbackContext context)
    {
        _rawMousePosition = context.ReadValue<Vector2>();
    }
    
    public T GetAttackData<T>() where T : AttackData
    {
        foreach (var data in attackLibrary)
        {
            if (data is T specificData) return specificData;
        }
        return null;
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    { 
        if (!context.performed) return;
        
        // Check the library for mouse attack data
        if (GetAttackData<MouseAttackData>() == null) return;
        
        // Gather the context 
        CombatContext combatContext = CombatContext;
        combatContext.source = gameObject;
        combatContext.mousePosition = _mainCam.ScreenToWorldPoint(_rawMousePosition);
        combatContext.userPosition = transform.position;
        combatContext.facingDirection = transform.right;
        CombatContext = combatContext;
      

        
        // Execute attack module
        _playerController.StateMachine.ChangeState(_playerController.MouseAttackState);
    }
}
