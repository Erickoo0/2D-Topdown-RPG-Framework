using UnityEngine;

[System.Serializable]
public class EntityChargeAttackState : BaseActionState
{
    private ChargeAttackData _attackData;
    private HitBox _spawnedHitbox;
    
    private float _windUpTimer;
    private float _chargeTimer;
    private Vector2 _chargeDirection;
    private float _originalSpeed;
    private bool _isFinished;
    private bool _hasHitThisExecute;

    public override void Setup(EntityController controller, StateMachine stateMachine)
    {
        base.Setup(controller, stateMachine);
        
        // Grab the attack data from the controllers attack library
        _attackData = controller.GetAttackData<ChargeAttackData>();
        
        if (_attackData == null) 
            Debug.LogError($"ChargeAttackData not found in attack library for {controller.gameObject.name}");
    }

    public override void Enter()
    {
        // 1. Reset state flags
        _isFinished = false;
        _hasHitThisExecute = false;
        _originalSpeed = controller.EntityMover.moveSpeed;
        
        // 2. Freeze and set timers
        controller.EntityMover.SetMoveDirection(Vector2.zero);
        _windUpTimer = _attackData.windUpDuration;

        // 3. Target Validation & Direction Logic
        if (controller.currentTarget != null)
        {
            Vector2 direction = (Vector2)controller.currentTarget.position - (Vector2)controller.transform.position;
            _chargeDirection = direction.normalized;

            float totalDistance = direction.magnitude + _attackData.overshootDistance;
            float totalChargeSpeed = _originalSpeed * _attackData.chargeSpeedMultiplier;
            _chargeTimer = totalDistance / totalChargeSpeed;
            
            controller.EntityAnimator.FaceDirection(_chargeDirection);
        }
        else 
        {
            _isFinished = true; 
            return;
        }

        // 4. Dynamic Hitbox Spawning & Initialization
        if (_attackData.hitboxPrefab != null)
        {
            // Spawn as child so it moves with the entity automatically
            _spawnedHitbox = Object.Instantiate(_attackData.hitboxPrefab, controller.transform);
            _spawnedHitbox.enableHitbox = false; 

            // --- APPLY OFFSET AND RADIUS HERE ---
            // LocalPosition ensures it stays at the offset relative to the parent
            _spawnedHitbox.transform.localPosition = _attackData.hitboxOffset;

            if (_spawnedHitbox is HitBoxCircle circleHitbox) 
            { 
                circleHitbox.radius = _attackData.hitboxRadius; 
            }
        }
    }

    public override void Update()
    {
        if (_isFinished)
        {
            stateMachine.ChangeState(controller.IdleState);
            return;
        }

        // Phase A: Windup
        if (_windUpTimer > 0)
        {
            _windUpTimer -= Time.deltaTime;
        }
        // Phase B: Active Charge
        else if (_chargeTimer > 0)
        {
            _chargeTimer -= Time.deltaTime;
            
            controller.EntityMover.moveSpeed = _originalSpeed * _attackData.chargeSpeedMultiplier;
            controller.EntityMover.SetMoveDirection(_chargeDirection);

            ProcessCombatLogic();
        }
        // Phase C: Completion
        else
        {
            _isFinished = true;
        }
    }

    private void ProcessCombatLogic()
    {
        if (_spawnedHitbox == null) return;

        // Hitbox is already at the correct offset via Enter(), just turn it on
        _spawnedHitbox.enableHitbox = true;

        if (!_attackData.hitOncePerExecute || !_hasHitThisExecute)
        {
            DamageData frameDamage = _attackData.damageData;
            frameDamage.source = controller.gameObject;

            if (_spawnedHitbox.CheckForHits(frameDamage))
            {
                _hasHitThisExecute = true;
            }
        }
    }

    public override void Exit()
    {
        controller.EntityMover.moveSpeed = _originalSpeed;
        controller.currentTarget = null;
        controller.EntityMover.SetMoveDirection(Vector2.zero);
        
        controller.SetActionCooldown();
        
        if (_spawnedHitbox != null)
        {
            Object.Destroy(_spawnedHitbox.gameObject);
        }
    }

    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
}