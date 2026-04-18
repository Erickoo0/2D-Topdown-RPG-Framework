using UnityEngine;

public class PlayerMoveState : State<PlayerController>
{
    public PlayerMoveState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) {}

    public override void Update()
    {
        Vector2 input = controller.MovementInput;

        if (input == Vector2.zero)
        {
            stateMachine.ChangeState(controller.IdleState);
            return;
        }
        
        controller.EntityMover.SetMoveDirection(input);
        
        // Check for dash input
        if (controller.dashInput == true)
        {
            stateMachine.ChangeState(controller.DashState);
        }
    }
    
    // Empty Methods
    public override void Enter() { }
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
    public override void Exit() { }
}
