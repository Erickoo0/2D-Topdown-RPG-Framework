using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Item/Consumable")]
public class ItemDataSoConsumable : ItemDataSo
{
    [Header("Hp Restoration")]
    public float hpHealAmount;
    public float hpHealDuration;

    [Header("Mp Restoration")]
    public float mpHealAmount;
    public float mpHealDuration;
    
    public override bool Use(ItemInstance itemInstance, GameObject target = null)
    {
        if (target == null) target = PlayerStatsManager.Instance.gameObject;
        bool hpRestored = HpHealCheck(target);
        bool mpRestored = MpHealCheck(target);
            
        return hpRestored || mpRestored;
    }
    
    private bool HpHealCheck(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        if (health == null) return false;
        if (hpHealAmount == 0) return false;
        
        if (health.HpCurrent >= health.hpMax && hpHealAmount > 0)
        {
            Debug.Log("MP is full, item not used!");
            return false;
        }
        
        // Check if heal over time
        if (hpHealDuration > 0)
        {
            // Exit if heal over time already exists
            if (health.HpIsHealingOverTime && hpHealAmount > 0)
            {
                 Debug.Log("HP is already healing! Wait for it to finish.");
                return false;
            }
            health.HpHealOverTime(hpHealAmount, hpHealDuration);
        }
        // If not heal over time, then instantly.
        else
        {
            health.HpHealInstant(hpHealAmount);
        }
        
        return true; // Successfully healed hp
    }
    
    private bool MpHealCheck(GameObject target)
    {
        Mana mana = target.GetComponent<Mana>();
        if (mana == null) return false;
        if (mpHealAmount == 0) return false;

        if (mana.MpCurrent >= mana.mpMax && mpHealAmount > 0)
        {
            Debug.Log("MP is full, item not used!");
            return false;
        }
        
        // Check if heal over time
        if (mpHealDuration > 0)
        {
            // Exit if heal over time already exists
            if (mana.MpIsHealingOverTime && mpHealAmount > 0) // Assuming the Mana script mirrors this property
            {
                Debug.Log("MP is already restoring! Wait for it to finish.");
                return false;
            }
            mana.MpHealOverTime(mpHealAmount, mpHealDuration);
        }
        // If not heal over time, then instant
        else
        {
            mana.MpHealInstant(mpHealAmount);
        }
        
        return true; // Successfully healed mp
    }
}

