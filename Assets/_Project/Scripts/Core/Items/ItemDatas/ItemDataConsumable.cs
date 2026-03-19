using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Item/Consumable")]
public class ItemDataConsumable : ItemData
{
    [Header("Hp Restoration")]
    public float hpHealAmount;
    public float hpHealDuration;

    [Header("Mp Restoration")]
    public float mpHealAmount;
    public float mpHealDuration;
    
    public override bool Use(GameObject user, ItemInstance itemInstance)
    {
        if (user == null) return false;
        bool hpRestored = HpHealCheck(user);
        bool mpRestored = MpHealCheck(user);
            
        return hpRestored || mpRestored;
    }
    
    private bool HpHealCheck(GameObject user)
    {
        if (hpHealAmount <= 0) return false;
        
        Health health = user.GetComponent<Health>();
        if (health == null) return false;
        if (health.HpCurrent >= health.hpMax)
        {
            Debug.Log("MP is full, item not used!");
            return false;
        }
        
        // Check if heal over time
        if (hpHealDuration > 0)
        {
            // Exit if heal over time already exists
            if (health.HpIsHealingOverTime)
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
    
    private bool MpHealCheck(GameObject user)
    {
        if (mpHealAmount <= 0) return false;
        
        Mana mana = user.GetComponent<Mana>();
        if (mana == null) return false;
        if (mana.MpCurrent >= mana.mpMax)
        {
            Debug.Log("MP is full, item not used!");
            return false;
        }
        
        // Check if heal over time
        if (mpHealDuration > 0)
        {
            // Exit if heal over time already exists
            if (mana.MpIsHealingOverTime) // Assuming the Mana script mirrors this property
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

