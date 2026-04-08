using UnityEngine;

public class MouseAttackModule : AttackModule
{
    public override void Execute(CombatContext combatContext)
    {
        hitbox.transform.position = combatContext.mousePosition;
        
        if (baseDamageData.source == null) baseDamageData.source = combatContext.attacker;
        
        hitbox.CheckForHits(baseDamageData);
        
        if (attackFX != null)
        {
            // Create the attackFX
           GameObject fxInstance = Instantiate(attackFX, combatContext.mousePosition, Quaternion.identity);
           
           // Try to get the FX script and sync the size
           if (fxInstance.TryGetComponent<FlashExplosionFX>(out FlashExplosionFX fx))
           {
               if (hitbox is HitBoxCircle circlehitbox)
               {
                   fx.SetupExplosion(circlehitbox.radius);
               }
           }
        }
    }
}
