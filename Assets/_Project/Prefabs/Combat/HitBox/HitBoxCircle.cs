using UnityEngine;

public class HitBoxCircle : HitBox
{
    [HideInInspector] public float radius;

    public override bool CheckForHits(DamageData data)
    {
        // Check collisions in a circle and asign to an array
        bool hitSucess = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, victimLayer);
        Debug.Log("Hit Check Started");

        // For every collision in the array, damage them
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == data.source) continue;
        
            // 1. Calculate a direction for knockback
            Vector2 targetPosition = hit.transform.position;
            Vector2 attackPosition = transform.position;
            Vector2 knockbackDirection = (targetPosition - attackPosition).normalized;
            // If the explosion is exactly on top of the enemy, knock them "up" or "away" by default
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up;
            }

            // 2. Create a copy of passed in damage data and modify the direction
            DamageData finalData = data;
            finalData.hitDirection = knockbackDirection;

            // 3. send the damage data
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
