using UnityEngine;

public class MouseAttackModule : AttackModule
{
    protected override float AttackSize => 0.9f;
    
    public override void EnterAttack(CombatContext combatContext)
    {
        // Pass the source 
        if (baseDamageData.source == null) baseDamageData.source = combatContext.source;
        
        if (hitbox is HitBoxCircle hitboxCircle) hitboxCircle.radius = AttackSize;
        
        TriggerVisuals(combatContext.mousePosition);
        
    }

    public override void UpdateAttack(CombatContext combatContext)
    {
        hitbox.transform.position = combatContext.mousePosition;
        hitbox.CheckForHits(baseDamageData);
    }

    private void TriggerVisuals(Vector2 position)
    {
        if (attackFX == null) return;
        
        GameObject fxInstance = Instantiate(attackFX, position, Quaternion.identity);
        
        hitbox.ScaleVisual(fxInstance);
    }
}
