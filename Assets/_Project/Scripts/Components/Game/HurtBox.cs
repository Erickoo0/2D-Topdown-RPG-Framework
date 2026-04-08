using UnityEngine;

[RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
public class HurtBox : MonoBehaviour, IDamagable
{
    [SerializeField] private Health health;
    //private Animator _animator;

    private void Awake()
    {
        if (health == null) health = GetComponent<Health>();
        //_animator = GetComponent<Animator>();
    }

    public void TakeDamage(DamageData data)
    {
        health.HpCurrent -= data.damageAmount;
        
        //_animator.SetTrigger("Hurt");
        
        ApplyKnockback(data.hitDirection, data.knockbackForce);
    }

    public void ApplyKnockback(Vector2 direction, float knockbackForce)
    {
        // Knockback logic later
    }
}
