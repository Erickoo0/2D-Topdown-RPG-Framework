using UnityEngine;

[ CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/Consumable")]
public class ItemDataConsumable : ItemData
{
    [Header("Consumable Effects")]
    public int healthHealAmount = 10;
    public int healthHealCount = 1;
    public int healthHealDuration = 0;
    public int manaHealAmount = 0;
    public int manaHealCount = 1;
    public int manaHealDuration = 0;

    public override bool Use(GameObject user, ItemInstance itemInstance)
    {
        var target = user.GetComponent<Health>();
        if (target == null) return false;
        if (healthHealAmount > 0)
        {
            // instant Heal
            if (healthHealDuration <= 0 || healthHealCount <= 1)
            {
                target.HealthHealInstant(healthHealAmount);
            }
            // Heal over time
            else
            {
                target.HealthHealOverTime(healthHealAmount, healthHealDuration, healthHealCount);
            }
        }

        if (manaHealAmount > 0)
        {
            if (manaHealDuration <= 0 || manaHealCount <= 1)
            {
                target.HealMana(manaHealAmount);
            }
            else
            {
                target.ManaHealOverTime(manaHealAmount, manaHealDuration, manaHealCount);
            }
        }

        return true;
    }
}
