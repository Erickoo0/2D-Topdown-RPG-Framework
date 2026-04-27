using UnityEngine;

[System.Serializable]
public class PlayerDashState : State<PlayerController>
{
    private float _dashTime;
    private float _defaultMoveSpeed;

    private float _afterImageTimer;
    private float _afterImageInterval = 0.04f;
    
    public override void Enter()
    {
        _dashTime = controller.defaultDashTime;
        _defaultMoveSpeed = controller.EntityMover.moveSpeed;
        controller.EntityMover.moveSpeed *= 5f;
        
        _afterImageTimer = 0;
    }
    
    public override void Update()
    {
        Vector2 input = controller.MovementInput;
        
        controller.EntityMover.SetMoveDirection(input);
        
        if (_afterImageTimer <= 0)
        {
            SpawnAfterImage();
            _afterImageTimer = _afterImageInterval;
        }
        
        _afterImageTimer -= Time.deltaTime;
        
    }

    public override void PhysicsUpdate()
    {
        if (_dashTime > 0) _dashTime--;
        else stateMachine.ChangeState(controller.IdleState);
    }
    
    public override void HandleInput() { }

    public override void Exit()
    {
        controller.EntityMover.moveSpeed = _defaultMoveSpeed;
        controller.dashInput = false; // Reset the bool
    }
    
    private void SpawnAfterImage()
    {
        GameObject player = controller.gameObject;
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        AfterImageManager.Instance.SpawnAfterImage(spriteRenderer.sprite, player.transform.position);
    }
}
