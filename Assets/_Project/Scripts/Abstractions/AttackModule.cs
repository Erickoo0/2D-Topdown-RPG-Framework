using UnityEngine;

public abstract class AttackModule : MonoBehaviour
{ 
    [SerializeField] protected HitBox hitbox;
    [SerializeField] protected GameObject attackFX;
    [SerializeField] protected DamageData baseDamageData;
    
    public abstract void Execute(CombatContext combatContext);
}

public struct CombatContext
{
    public GameObject attacker;
    public Vector2 mousePosition;
    public Vector2 userPosition;
    public Vector2 facingDirection;
}