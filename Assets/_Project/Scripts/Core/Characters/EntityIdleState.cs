using UnityEngine;

public class EntityIdleState : State<EntityController>
{
    public EntityIdleState(EntityController context, StateMachine stateMachine) : base(context, stateMachine) { }

    private float _idleTime;
    
    
    public override void Enter()
    {
        context.EntityMover.SetMoveDirection(Vector2.zero); 
        _idleTime = Random.Range(100f, 300f);
    }

    public override void Update()
    {
        if (context.IsTargetInRange())
        {
            //stateMachine.ChangeState(context.ChaseState);
        }
    }
    
    public override void PhysicsUpdate() 
    {
        if (_idleTime > 0) _idleTime--;
        else stateMachine.ChangeState(context.WanderState);
    }
    
    public override void HandleInput() { }
    
    public override void Exit() { }
}
