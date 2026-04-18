using UnityEngine;

public abstract class HitBox : MonoBehaviour
{
    [Header("Base Settings")] 
    public LayerMask victimLayer; // Layer to check for collisions
    public bool enableHitbox = true;
    
    // Every hitbox must implement this and define their own logic to check for hits
    public abstract bool CheckForHits(DamageData data);

    public abstract void ScaleVisual(GameObject attackFX);

    protected bool SendDamage(DamageData data, Collider2D victimCollider)
    {
        if (!enableHitbox) return false;
        
        if (victimCollider.TryGetComponent<IDamagable>(out IDamagable victim))
        {
            victim.TakeDamage(data);
            return true; // Successfully hit an IDamagable
        }

        return false; // It was on the layer mask, but not an IDamagable
    }
}
