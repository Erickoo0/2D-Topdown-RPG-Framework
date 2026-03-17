using UnityEngine;
using System;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    public event Action<int> OnManaChanged;

    public int manaMax = 100;
    
    private int _manaCurrent = 10;

    // Mana Property
    public int ManaCurrent
    {
        get => _manaCurrent;
        set
        {
            int previousMana = _manaCurrent;
            _manaCurrent = Mathf.Clamp(value, 0, manaMax);
            OnManaChanged?.Invoke(_manaCurrent);
        }
    }

    private void Start()
    {
        //_manaCurrent = manaMax;
    }

    public void HealMana(int amount)
    {
        if (amount <= 0) return;
        ManaCurrent += amount;
        Debug.unityLogger.Log($"{gameObject.name}: Health healed by {amount}");
    }

    public void TakeManaDamage(int amount)
    {
        if (amount <= 0) return;
        ManaCurrent -= amount;
        Debug.unityLogger.Log($"{gameObject.name}: Mana damaged by {amount}");
    }   
}