using UnityEngine;
public class PlayerDashState : State<PlayerController>
{
    public PlayerDashState(PlayerController context, StateMachine stateMachine) : base(context, stateMachine) { }

    public float dashTime;
    private float _defaultMoveSpeed;
    public override void Enter()
    {
        dashTime = context.defaultDashTime;
        _defaultMoveSpeed = context.EntityMover.moveSpeed;
        context.EntityMover.moveSpeed *= 5f;
    }
    public override void Update()
    {
        Vector2 input = context.MovementInput;
        
        context.EntityMover.SetMoveDirection(input);
    }

    public override void PhysicsUpdate()
    {
        if (dashTime > 0) dashTime--;
        else stateMachine.ChangeState(context.IdleState);
    }
    
    public override void HandleInput() { }

    public override void Exit()
    {
        context.EntityMover.moveSpeed = _defaultMoveSpeed;
        context.dashInput = false; // Reset the bool
    }
}
