using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
public class HurtBox : MonoBehaviour, IDamagable
{
    [Tooltip("Leave empty if parent object")]
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
        
        if (TryGetComponent<EntityMover>(out EntityMover entityMover))
        {
            entityMover.ApplyKnockback(data.hitDirection, data.knockbackForce, data.knockbackDuration, data.source);
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();  
        }
        
        // Apply flash shader
        if (TryGetComponent<FlashShader>(out FlashShader flashShader))
        {
            flashShader.ApplyFlash();
        }
    }
}
