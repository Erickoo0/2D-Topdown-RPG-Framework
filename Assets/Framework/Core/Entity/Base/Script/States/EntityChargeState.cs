using UnityEngine;

[System.Serializable]
public class EntityChargeState : State<EntityController>
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
        
        _attackData = controller.GetAttackData<ChargeAttackData>();
        
        if (_attackData == null) 
            Debug.LogError("ChargeAttackData not found in attack library for EntityChargeState");
    }

    public override void Enter()
    {
        // 1. Reset state flags
        _isFinished = false;
        _hasHitThisExecute = false;
        _originalSpeed = controller.EntityMover.moveSpeed;
        _chargeTimer = 0f;
        _windUpTimer = _attackData.windUpDuration;
        
        // 2. Freeze and set timers
        controller.EntityMover.SetMoveDirection(Vector2.zero);
        _windUpTimer = _attackData.windUpDuration;

        // 3. Target Validation & Direction Logic
        if (controller.currentTarget != null)
        {
            Vector2 direction = (Vector2)controller.currentTarget.position - (Vector2)controller.transform.position;
            _chargeDirection = direction.normalized;

            // Calculate charge duration based on distance to target + overshoot constant
            float totalDistance = direction.magnitude + _attackData.overshootDistance;
            float speed = _originalSpeed * _attackData.chargeSpeedMultiplier;
            _chargeTimer = totalDistance / speed;
            
            controller.EntityAnimator.FaceDirection(_chargeDirection);
        }
        else
        {
            _isFinished = true; // Bail if target is lost before starting
            return;
        }

        // 4. Dynamic Hitbox Spawning
        if (_attackData.hitboxPrefab != null)
        {
            // Spawn the hitbox prefab as a child of the entity
            _spawnedHitbox = Object.Instantiate(_attackData.hitboxPrefab, controller.transform);
            _spawnedHitbox.enableHitbox = false; // Keep it "cold" during the windup phase

            // If it's a circle hitbox, apply the radius from the ScriptableObject
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

        // --- Phase A: Windup ---
        if (_windUpTimer > 0)
        {
            _windUpTimer -= Time.deltaTime;
        }
        // --- Phase B: Active Charge ---
        else if (_chargeTimer > 0)
        {
            _chargeTimer -= Time.deltaTime;
            
            // Apply movement
            controller.EntityMover.moveSpeed = _originalSpeed * _attackData.chargeSpeedMultiplier;
            controller.EntityMover.SetMoveDirection(_chargeDirection);

            ProcessCombatLogic();
        }
        // --- Phase C: Completion ---
        else
        {
            _isFinished = true;
        }
    }

    private void ProcessCombatLogic()
    {
        if (_spawnedHitbox == null) return;

        // Ensure the hitbox follows any specific offset defined in the data
        _spawnedHitbox.transform.localPosition = _attackData.hitboxOffset;
        _spawnedHitbox.enableHitbox = true;
        Debug.Log("Hitbox enabled");

        // Check if we can still hit (Handles 'Hit Once' logic)
        if (!_attackData.hitOncePerExecute || !_hasHitThisExecute)
        {
            // Use the Data Asset's damage info, but assign this specific entity as the source
            DamageData frameDamage = _attackData.damageData;
            frameDamage.source = controller.gameObject;

            if (_spawnedHitbox.CheckForHits(frameDamage))
            {
                _hasHitThisExecute = true;
                Debug.Log("Hit!");
                // Add impact freeze or camera shake triggers here if desired
            }
        }
    }

    public override void Exit()
    {
        // 1. Restore original movement speed
        controller.EntityMover.moveSpeed = _originalSpeed;
        controller.EntityMover.SetMoveDirection(Vector2.zero);
        
        // 2. Start the cooldown timer on the controller
        controller.SetActionCooldown();
        
        // 3. Clean up the dynamic hitbox
        if (_spawnedHitbox != null)
        {
            Object.Destroy(_spawnedHitbox.gameObject);
        }
    }

    // Unused in the context of a programmed Charge, but required by State interface
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
}