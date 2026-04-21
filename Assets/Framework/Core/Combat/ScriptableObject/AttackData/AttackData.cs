using UnityEngine;

public abstract class AttackData : ScriptableObject
{
    [Header("Hitbox Reference")] 
    public HitBox hitboxPrefab; // The specific prefab for this attack
    
    [Header("Combat Settings")] 
    public DamageData damageData;
    public float hitboxRadius = 1.0f;
    public Vector2 hitboxOffset;
    public bool hitOncePerExecute = true;

}
