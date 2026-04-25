using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseEntityController
{
    [Header("Player Data")]
    public Vector2 MovementInput { get; private set; }
    public float defaultDashTime = 10f;
    
    [Header("State References")]
    [SerializeReference, SubclassSelector] public State<PlayerController> IdleState;
    [SerializeReference, SubclassSelector] public State<PlayerController> MoveState;
    [SerializeReference, SubclassSelector] public State<PlayerController> DashState;
    [SerializeReference, SubclassSelector] public State<PlayerController> MouseAttackState;

    [HideInInspector] public bool dashInput;
    private bool _canMove = true;
    private Vector2 _rawInput;

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
        // Always track input even if player is not moving
        _rawInput = context.ReadValue<Vector2>();
        
        // Only update movement input if player can move
        if (_canMove) 
            MovementInput = _rawInput;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) dashInput = true;
    }
    
    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        
        // Clear input for animator and state machine
        if (!_canMove)
        {
            MovementInput = Vector2.zero;
            EntityMover.SetMoveDirection(Vector2.zero);
            dashInput = false;
        }
        else
        {
            MovementInput = _rawInput;
        }
    }
}
