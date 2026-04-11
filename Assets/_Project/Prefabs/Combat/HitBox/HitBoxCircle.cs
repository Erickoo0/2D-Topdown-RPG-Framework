using UnityEngine;

public class HitBoxCircle : HitBox
{
    // Base 0.9 matches FlashExplosionFX sprite with a scale of 1
    [HideInInspector] public float radius;

    public override void CheckForHits(DamageData data)
    {
        // Check collisions in a circle and asign to an array
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, victimLayer);
        
        // For every collision in the array, damage them
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == data.source) continue;
        
            // 1. Calculate direction for knockback
            Vector2 targetPosition = hit.transform.position;
            Vector2 attackPosition = transform.position;
            Vector2 knockbackDirection = (targetPosition - attackPosition).normalized;
            // If the explosion is exactly on top of the enemy, knock them "up" or "away" by default
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up;
            }

            // 2. Create a copy of passed in damage data, and modify direction
            DamageData finalData = data;
            finalData.hitDirection = knockbackDirection;

            // 3. send the damage data
            SendDamage(finalData, hit);
            
        }
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
