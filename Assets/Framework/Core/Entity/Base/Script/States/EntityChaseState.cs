using UnityEngine;

[System.Serializable]
public class EntityChaseState : State<EntityController>
{
    public override void Enter() { }

    public override void Update()
    {
        Vector2 currentPosition = controller.transform.position;
        Vector2 targetPosition = controller.currentTarget.position;
        
        float distance = Vector2.Distance(currentPosition, targetPosition);
        
        // If in action range and action cooldown is over, switch to the action state
        if (distance <= controller.ActionRange && controller.CheckActionCooldown())
        {
            stateMachine.ChangeState(controller.ChargeState);
        }
        else // Keep Chasing
        {
            Vector2 moveDirection = (targetPosition - currentPosition).normalized;
            controller.EntityMover.SetMoveDirection(moveDirection);
        }
    }
    
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }

    public override void Exit()
    {
        // Stop moving when leaving chase state so entity does not slide when transitioning
        controller.EntityMover.SetMoveDirection(Vector2.zero);
    }
}
