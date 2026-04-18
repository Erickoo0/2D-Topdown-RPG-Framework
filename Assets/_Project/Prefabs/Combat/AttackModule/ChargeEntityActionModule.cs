using UnityEngine;

public class ChargeEntityActionModule : EntityActionModule
{
    [Header("Charge Settings")]
    [SerializeField] private float windUpDuration = 0.75f;
    [SerializeField] private float chargeSpeedMultiplier = 4f;
    [SerializeField] private float overshootDistance = 15f;
    [SerializeField] private ChargeAttackModule chargeAttackModule;
    
    private float _windUpTimer;
    private float _chargeTimer;
    private Vector2 _chargeDirection;
    private float _originalSpeed;
    private bool _isFinished;
    private CombatContext _combatContext;
    
    public bool IsCharging { get; private set; }
    
    public override void EnterAction(EntityController controller)
    {
        _isFinished = false;
        IsCharging = false;
        
        _originalSpeed = controller.EntityMover.moveSpeed; // Save the original speed
        controller.EntityMover.SetMoveDirection(Vector2.zero);
        
        // Set the timers
        _windUpTimer = windUpDuration; 
        _chargeTimer = 0f;

        if (controller.currentTarget != null)
        {
            Vector2 direction = controller.currentTarget.position - controller.transform.position;
            float distanceToTarget = direction.magnitude;
            _chargeDirection = direction.normalized;

            float totalChargeDistance = distanceToTarget + overshootDistance;
            float travelSpeed = _originalSpeed * chargeSpeedMultiplier;
            // Calculate charge timer based on distance and speed
            _chargeTimer = totalChargeDistance / travelSpeed;
            
            controller.EntityAnimator.FaceDirection(_chargeDirection);
        }
        else // If there is no target, instantly set to finished
        {
            _isFinished = true;
        }

        // Create the combat context
        if (chargeAttackModule != null)
        {
            _combatContext = new CombatContext
            {
                source = controller.gameObject,
                entityActionModule = this,
                userPosition = controller.transform.position,
                facingDirection = _chargeDirection,
                mousePosition = controller.transform.position
            };
            
            chargeAttackModule.EnterAttack(_combatContext);
        }
    }
    
    public override void UpdateAction(EntityController controller)
    {
        if (_isFinished) return;
        
        // Wind up the charge
        if (_windUpTimer > 0f)
        {
            _windUpTimer -= Time.deltaTime;
            IsCharging = false;
            controller.EntityMover.SetMoveDirection(Vector2.zero);
        }
        else 
        {
            if (_chargeTimer > 0f)
            {
                _chargeTimer -= Time.deltaTime;
                IsCharging = true; 
            
                controller.EntityMover.moveSpeed = _originalSpeed * chargeSpeedMultiplier;
                controller.EntityMover.SetMoveDirection(_chargeDirection);
            }
            else
            {
                IsCharging = false;
                _isFinished = true;
            }
        }
        
        // Update the combat context and hitbox
        if (chargeAttackModule != null)
        {
            _combatContext.userPosition = controller.transform.position;
            
            // Call the attack module
            chargeAttackModule.UpdateAttack(_combatContext);
        }
    }
    
    public override bool IsFinishedAction(EntityController context)
    {
        return _isFinished;
    }
    
    public override void ExitAction(EntityController context)
    {
        context.EntityMover.moveSpeed = _originalSpeed;
        IsCharging = false;
        context.EntityMover.SetMoveDirection(Vector2.zero);
        
        if (chargeAttackModule != null)
        {
            chargeAttackModule.ExitAttack(_combatContext);
        }
    }
}
