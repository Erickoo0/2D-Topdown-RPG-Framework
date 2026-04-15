using UnityEngine;

public class ChargeAttackModule : AttackModule
{
    [SerializeField] private Vector2 hitboxOffset = Vector2.zero;
    [SerializeField] private float chargeHitDuration = 0.2f;
    [SerializeField] private bool hitOncePerExecute = true;

    private bool _isExecuting;
    private bool _hasHitThisExecute;
    private float _endTime;

    public void BeginCharge(CombatContext combatContext)
    {
        if (combatContext.source == null) return;

        _isExecuting = true;
        _hasHitThisExecute = false;
        _endTime = Time.time + chargeHitDuration;

        if (baseDamageData.source == null)
            baseDamageData.source = combatContext.source;

        hitbox.transform.position = (Vector2)combatContext.source.transform.position + hitboxOffset;
        if (hitbox is HitBoxCircle hitboxCircle) hitboxCircle.radius = attackSize;
    }

    private void Update()
    {
        if (!_isExecuting) return;

        if (hitbox != null)
            hitbox.transform.position = transform.position + (Vector3)hitboxOffset;

        if (!_hasHitThisExecute || !hitOncePerExecute)
        {
            hitbox.CheckForHits(baseDamageData);
            _hasHitThisExecute = true;
        }

        if (Time.time >= _endTime)
            _isExecuting = false;
    }

    public override void Execute(CombatContext combatContext)
    {
        BeginCharge(combatContext);
    }
}