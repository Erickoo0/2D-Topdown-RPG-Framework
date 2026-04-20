using UnityEngine;

[System.Serializable]
public class EntityIdleState : State<EntityController>
{
    private float _idleTime;
    
    
    public override void Enter()
    {
        controller.EntityMover.SetMoveDirection(Vector2.zero); 
        _idleTime = Random.Range(100f, 300f);
    }

    public override void Update()
    {
        if (_idleTime > 0) _idleTime -= Time.deltaTime;
        else stateMachine.ChangeState(controller.WanderState);
    }
    
    public override void PhysicsUpdate() { }
    
    public override void HandleInput() { }
    
    public override void Exit() { }
}
