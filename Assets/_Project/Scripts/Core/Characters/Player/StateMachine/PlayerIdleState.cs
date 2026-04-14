using UnityEngine;

public class PlayerIdleState : State<PlayerController>
{
    public PlayerIdleState(PlayerController context, StateMachine stateMachine) : base(context, stateMachine) { }
    
    public override void Enter() { context.EntityMover.SetMoveDirection(Vector2.zero); }

    public override void Update()
    {
        Vector2 input = context.MovementInput;

        if (input != Vector2.zero)
            stateMachine.ChangeState(context.MoveState);
        else
            context.EntityMover.SetMoveDirection(Vector2.zero);
    }
    
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
    public override void Exit() { }
}
