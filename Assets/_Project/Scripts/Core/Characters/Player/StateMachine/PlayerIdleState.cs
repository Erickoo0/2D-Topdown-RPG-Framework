using UnityEngine;

public class PlayerIdleState : State<PlayerController>
{
    public PlayerIdleState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) { }
    
    public override void Enter() { controller.EntityMover.SetMoveDirection(Vector2.zero); }

    public override void Update()
    {
        Vector2 input = controller.MovementInput;

        if (input != Vector2.zero)
            stateMachine.ChangeState(controller.MoveState);
        else
            controller.EntityMover.SetMoveDirection(Vector2.zero);
    }
    
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
    public override void Exit() { }
}
