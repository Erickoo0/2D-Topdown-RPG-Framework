using UnityEngine;

public class EntityActionState : State<EntityController>
{
    private readonly IEntityAction _entityAction;

    public EntityActionState(EntityController controller, StateMachine stateMachine, IEntityAction entityAction) : base(controller, stateMachine)
    {
        // Pass the action from sonstructor (in controller) to the state
        _entityAction = entityAction;
    }

    public override void Enter()
    {
        controller.EntityMover.SetMoveDirection(Vector2.zero);
        _entityAction?.EnterAction(controller); // Call the action's Enter method'
    }

    public override void Update()
    {
        // Safety Check
        if (_entityAction == null)
        {
            stateMachine.ChangeState(controller.IdleState);
            return;
        }

        // Check if the action is finished
        if (_entityAction.IsFinishedAction(controller))
        {
            stateMachine.ChangeState(controller.IdleState);
            return;
        }
        
        _entityAction.UpdateAction(controller); // Call the action's Update method'
    }
    
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }

    public override void Exit()
    {
        _entityAction?.ExitAction(controller); // Call the action's Exit method'
        controller.SetActionCooldown(); // Reset the cooldown timer
    }
    
}
