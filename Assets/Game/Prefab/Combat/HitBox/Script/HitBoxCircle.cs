using UnityEngine;

public class HitBoxCircle : HitBox
{
    public float radius;

    public override bool CheckForHits(DamageData data)
    {
        if (!enableHitbox) return false;
        
        // Check collisions in a circle and asign to an array
        bool hitSucess = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, victimLayer);

        // For every collision in the array, damage them
        foreach (Collider2D hit in hits)
        {
            // Skip self
            if (hit.gameObject == data.source) continue;
        
            // 1. Calculate a direction for knockback
            Vector2 targetPosition = hit.transform.position;
            Vector2 attackPosition = transform.position;
            Vector2 primaryKnockbackDirection = (targetPosition - attackPosition).normalized;
            
            // 2. Calculate perpendicular offset
            Vector2 perpendicularDirection = new Vector2(-primaryKnockbackDirection.y, primaryKnockbackDirection.x); 
            
            // 3. Determine the Side
            float sideSign = (primaryKnockbackDirection.x * perpendicularDirection.y - primaryKnockbackDirection.y * perpendicularDirection.x) > 0 ? 1f : -1f;   

            // 4. Pass the direction
            Vector2 finalDirection = (primaryKnockbackDirection + (perpendicularDirection * (sideSign * data.knockbackForce))).normalized;
            
            // 5. Create a copy of passed in damage data and direction
            DamageData finalData = data;
            finalData.hitDirection = finalDirection;

            // 6. send the damage data
            if (SendDamage(finalData, hit))
            {
                hitSucess = true;
            }
            
        }
        
        return hitSucess; // Retrurn the result for the module 
    }

    public override void ScaleVisual(GameObject attackFX)
    {
        if (attackFX.TryGetComponent<FlashExplosionFX>(out FlashExplosionFX fx))
        {
            fx.SetupExplosion(radius);
        }
    }
    
    private void OnDrawGizmos() { Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, radius); }
}
