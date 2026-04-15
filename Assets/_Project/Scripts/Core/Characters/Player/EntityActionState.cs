using UnityEngine;

public class EntityActionState : State<EntityController>
{
    private readonly IEntityAction _entityAction;

    public EntityActionState(EntityController context, StateMachine stateMachine, IEntityAction entityAction) : base(context, stateMachine)
    {
        _entityAction = entityAction;
    }

    public override void Enter()
    {
        context.EntityMover.SetMoveDirection(Vector2.zero);
        _entityAction?.Enter(context);
    }

    public override void Update()
    {
        // Safety Check
        if (_entityAction == null)
        {
            stateMachine.ChangeState(context.IdleState);
            return;
        }
        
        _entityAction.Update(context);

        if (_entityAction.IsFinished(context))
        {
            stateMachine.ChangeState(context.IdleState);
        }
    }
    
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }

    public override void Exit()
    {
        _entityAction?.Exit(context);
        context.ResetActionCooldown();
    }
    
}
