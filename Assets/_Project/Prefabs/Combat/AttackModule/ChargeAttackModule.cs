using UnityEngine;

public class ChargeAttackModule : AttackModule
{
    [Header("Charge Attack Settings")]
    [SerializeField] private Vector2 hitboxOffset = Vector2.zero;
    [SerializeField] private bool hitOncePerExecute = true;
    private ChargeEntityActionModule _entityActionModule;
    

    
    private bool _hasHitThisExecute = false;

    public override void EnterAttack(CombatContext combatContext)
    {
        _entityActionModule = combatContext.entityActionModule as ChargeEntityActionModule;
        if (baseDamageData.source == null) baseDamageData.source = combatContext.source;

        // Scale the hitbox
        if (hitbox is HitBoxCircle hitboxCircle) hitboxCircle.radius = AttackSize;
        
        _hasHitThisExecute = false;
        hitbox.enabled = false;
    }    
    public override void UpdateAttack(CombatContext combatContext)
    {
        if (_entityActionModule == null || hitbox == null) return;
        
        if (_entityActionModule.IsCharging)
        {
            hitbox.enabled = true;
            hitbox.transform.position = (Vector2)combatContext.userPosition + hitboxOffset;
            
            if (!hitOncePerExecute || !_hasHitThisExecute)
            {
                if (hitbox.CheckForHits(baseDamageData))
                { 
                    _hasHitThisExecute = true;
                }
            }
        }
        else
        {
            hitbox.enabled = false;
        }
    }

    public override void ExitAttack(CombatContext combatContext)
    {
        _hasHitThisExecute = false;

        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }
}