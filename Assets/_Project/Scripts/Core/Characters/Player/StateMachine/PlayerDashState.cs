using UnityEngine;
public class PlayerDashState : State<PlayerController>
{
    public PlayerDashState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) { }

    public float dashTime;
    private float _defaultMoveSpeed;
    public override void Enter()
    {
        dashTime = controller.defaultDashTime;
        _defaultMoveSpeed = controller.EntityMover.moveSpeed;
        controller.EntityMover.moveSpeed *= 5f;
    }
    public override void Update()
    {
        Vector2 input = controller.MovementInput;
        
        controller.EntityMover.SetMoveDirection(input);
    }

    public override void PhysicsUpdate()
    {
        if (dashTime > 0) dashTime--;
        else stateMachine.ChangeState(controller.IdleState);
    }
    
    public override void HandleInput() { }

    public override void Exit()
    {
        controller.EntityMover.moveSpeed = _defaultMoveSpeed;
        controller.dashInput = false; // Reset the bool
    }
}
