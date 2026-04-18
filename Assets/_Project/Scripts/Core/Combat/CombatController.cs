using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    [SerializeField] private AttackModule currentAttack;

    private Camera _mainCam;
    private Vector2 _rawMousePosition;

    private void Awake() => _mainCam = Camera.main;
    
    
    //Tracks Mouse Movement
    public void OnPoint(InputAction.CallbackContext context)
    {
        _rawMousePosition = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    { 
        
        // Gather the context 
        CombatContext contextData = new CombatContext
        {
            source = gameObject,
            mousePosition = _mainCam.ScreenToWorldPoint(_rawMousePosition),
            userPosition = transform.position,
            facingDirection = transform.right // Add logic later
        };
        
        // Execute attack module
        currentAttack.EnterAttack(contextData);
        currentAttack.UpdateAttack(contextData);
        Debug.Log($"{contextData.userPosition} : {contextData.source} : {contextData.mousePosition}");
    }
}
