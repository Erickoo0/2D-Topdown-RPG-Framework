using UnityEngine;

public class ChargeEntityActionModule : EntityActionModule
{
    [Header("Charge Settings")]
    [SerializeField] private float windUpDuration = 0.75f;
    [SerializeField] private float chargeSpeedMultiplier = 4f;
    [SerializeField] private float overshootDistance = 10f;
    [SerializeField] private ChargeAttackModule chargeAttackModule;
    
    private float _windUpTimer;
    private float _chargeTimer;
    private Vector2 _chargeDirection;
    private float _originalSpeed;
    private bool _isFinished;
    
    public override void Enter(EntityController context)
    {
        _isFinished = false;
        _originalSpeed = context.EntityMover.moveSpeed;
        context.EntityMover.SetMoveDirection(Vector2.zero);

        _windUpTimer = windUpDuration;

        if (context.currentTarget != null)
        {
            Vector2 offset = context.currentTarget.position - context.transform.position;
            float distanceToTarget = offset.magnitude;
            _chargeDirection = offset.normalized;

            float totalChargeDistance = distanceToTarget + overshootDistance;
            float travelSpeed = _originalSpeed * chargeSpeedMultiplier;
            _chargeTimer = totalChargeDistance / travelSpeed;

            context.EntityAnimator.FaceDirection(_chargeDirection);
        }
        else
        {
            _isFinished = true;
        }
    }
    
    public override void Update(EntityController context)
    {
        if (_isFinished) return;

        if (_windUpTimer > 0f)
        {
            _windUpTimer -= Time.deltaTime;
            context.EntityMover.SetMoveDirection(Vector2.zero);
            return;
        }

        if (_chargeTimer > 0f)
        {
            _chargeTimer -= Time.deltaTime;

            context.EntityMover.moveSpeed = _originalSpeed * chargeSpeedMultiplier;
            context.EntityMover.SetMoveDirection(_chargeDirection);

            if (chargeAttackModule != null)
            {
                CombatContext combatContext = new CombatContext
                {
                    source = context.gameObject,
                    userPosition = context.transform.position,
                    facingDirection = _chargeDirection,
                    mousePosition = context.transform.position
                };

                chargeAttackModule.Execute(combatContext);
            }
        }
        else
        {
            _isFinished = true;
        }
    }
    
    public override bool IsFinished(EntityController context)
    {
        return _isFinished;
    }
    
    public override void Exit(EntityController context)
    {
        context.EntityMover.moveSpeed = _originalSpeed;
        context.EntityMover.SetMoveDirection(Vector2.zero);
    }
}
