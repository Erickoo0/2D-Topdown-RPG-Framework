using UnityEngine;
public class EntityWanderState : State<EntityController>
{
    public EntityWanderState(EntityController context, StateMachine stateMachine) : base(context, stateMachine) { }
    
    public override void Enter() { }

    public override void Update()
    {
        Vector2 currentPosition = context.transform.position;
        Vector2 targetPosition = context.GetCurrentWaypointPosition();
        
        float distance = Vector2.Distance(currentPosition, targetPosition);
        
        // If we have not reached the destination, move towards it
        if (distance > 0.1f)
        {
            Vector2 moveDirection = (targetPosition - currentPosition).normalized;
            context.EntityMover.SetMoveDirection(moveDirection);
        }
        else // If we have reached the destination, return to idle state
        {
            context.AdvanceToNextWaypoint();
            stateMachine.ChangeState(context.IdleState);
        }
    }
    
    public override void PhysicsUpdate() { }

    public override void HandleInput() { }

    public override void Exit() { }
}
