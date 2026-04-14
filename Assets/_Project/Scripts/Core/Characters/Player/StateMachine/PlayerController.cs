using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseEntityController
{
    [Header("Player Data")]
    public Vector2 MovementInput { get; private set; }
    public bool dashInput;
    public float defaultDashTime = 10f;
    
    [Header("State References")]
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerDashState DashState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
    }

    protected virtual void Start()
    {
        // Default to the idle state
        StateMachine.SetupState(IdleState);
    }
    
    // Input System Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) dashInput = true;
    }
}
