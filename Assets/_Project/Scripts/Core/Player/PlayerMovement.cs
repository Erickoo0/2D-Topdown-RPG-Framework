using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EntityMover))]
public class PlayerController : MonoBehaviour
{
    private EntityMover _entityMover;
    
    private void Awake() => _entityMover = GetComponent<EntityMover>();
    
    public void Move(InputAction.CallbackContext context)
    {
        _entityMover.SetMoveDirection(context.ReadValue<Vector2>());
    }
}

