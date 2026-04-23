using UnityEngine;

[System.Serializable]
public class EntityIdleState : BaseIdleState
{
    private float _idleTime;
    
    
    public override void Enter()
    {
        controller.EntityMover.SetMoveDirection(Vector2.zero); 
        _idleTime = Random.Range(0.5f, 1.5f);
    }

    public override void Update()
    {
        if (_idleTime > 0) _idleTime -= Time.deltaTime;
        else stateMachine.ChangeState(controller.WanderState);
    }
    
    public override void PhysicsUpdate() { }
    
    public override void HandleInput() { }

    public override void Exit()
    {
        controller.EntityMover.SetMoveDirection(Vector2.zero);
    }
}
