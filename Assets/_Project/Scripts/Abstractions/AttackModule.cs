using UnityEngine;

public abstract class AttackModule : MonoBehaviour
{ 
    [SerializeField] protected HitBox hitbox;
    [SerializeField] protected GameObject attackFX;
    [SerializeField] protected DamageData baseDamageData;
    
    protected virtual float AttackSize => 1.0f;
    
    public virtual void EnterAttack(CombatContext combatContext) { }
    public virtual void UpdateAttack(CombatContext combatContext) { }
    public virtual void ExitAttack(CombatContext combatContext) { }

    public virtual bool IsFinished => false;
}
