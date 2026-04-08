using UnityEngine;

public class HitBoxCircle : HitBox
{
    // Base 0.9 matches FlashExplosionFX sprite with a scale of 1
    public float radius = 0.9f;

    public override void CheckForHits(DamageData data)
    {
        // Check collisions in a circle and asign to an array
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, victimLayer);
        
        // For every collision in the array, damage them
        foreach (Collider2D hit in hits) SendDamage(data, hit);
    }
    
    private void OnDrawGizmos() { Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, radius); }
}
