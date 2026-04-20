using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseEntityController
{
    [Header("Player Data")]
    public Vector2 MovementInput { get; private set; }
    public bool dashInput;
    public float defaultDashTime = 10f;
    
    [Header("State References")]
    [SerializeReference, SubclassSelector] public State<PlayerController> IdleState;
    [SerializeReference, SubclassSelector] public State<PlayerController> MoveState;
    [SerializeReference, SubclassSelector] public State<PlayerController> DashState;
    [SerializeReference, SubclassSelector] public State<PlayerController> MouseAttackState;

    protected override void Awake()
    {
        base.Awake();

        IdleState?.Setup(this, StateMachine);
        MoveState?.Setup(this, StateMachine);
        DashState?.Setup(this, StateMachine);
        MouseAttackState?.Setup(this, StateMachine);
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
