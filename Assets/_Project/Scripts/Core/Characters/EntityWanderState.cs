using UnityEngine;

public class EntityWanderState : State<EntityController>
{
    public EntityWanderState(EntityController controller, StateMachine stateMachine) : base(controller, stateMachine) { }
    
    public override void Enter() { }

    public override void Update()
    {
        Vector2 currentPosition = controller.transform.position;
        Vector2 targetPosition = controller.GetCurrentWaypointPosition();
        
        float distance = Vector2.Distance(currentPosition, targetPosition);
        
        // If we have not reached the destination, move towards it
        if (distance > 0.1f)
        {
            Vector2 moveDirection = (targetPosition - currentPosition).normalized;
            controller.EntityMover.SetMoveDirection(moveDirection);
        }
        else // If we have reached the destination, return to idle state
        {
            controller.AdvanceToNextWaypoint();
            stateMachine.ChangeState(controller.IdleState);
        }
    }
    
    public override void PhysicsUpdate() { }

    public override void HandleInput() { }

    public override void Exit()
    {
        controller.EntityMover.SetMoveDirection(Vector2.zero);
    }
}
