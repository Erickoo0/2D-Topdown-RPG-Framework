using UnityEngine;

public class PlayerMoveState : State<PlayerController>
{
    public PlayerMoveState(PlayerController context, StateMachine stateMachine) : base(context, stateMachine) {}

    public override void Update()
    {
        Vector2 input = context.MovementInput;

        if (input == Vector2.zero)
        {
            stateMachine.ChangeState(context.IdleState);
            return;
        }
        
        context.EntityMover.SetMoveDirection(input);
        
        // Check for dash input
        if (context.dashInput == true)
        {
            stateMachine.ChangeState(context.DashState);
        }
    }
    
    // Empty Methods
    public override void Enter() { }
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
    public override void Exit() { }
}
