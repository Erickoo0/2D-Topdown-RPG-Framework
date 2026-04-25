using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int lvlCurrent = 1;
    [SerializeField] private int lvlMax = 99;
    [SerializeField] private int expCurrent = 0;
    [SerializeField] private int baseExpOnDeath = 10;

    [Header("Experience Scaling Formula")]
    [SerializeField] private int baseExpRequirement = 100;
    [SerializeField] private float expScalingFactor = 1.5f;
    
    [Header("Designer Overrides")]
    [Tooltip("If true, ignores the formula and uses the curve below.")]
    [SerializeField] private bool useCustomCurve = false;
    [Tooltip("X-axis represents the Level. Y-axis represents the EXP required for that level.")]
    [SerializeField] private AnimationCurve customExpCurve;
    
    // Events
    public event Action<int> OnLevelUpdated;
    public event Action<int, int> OnExperienceGained; // Current EXP, EXP Needed for Next Level
    public event Action OnMaxLevelReached;
    
    // Properties
    public int LvlCurrent
    {
        get => lvlCurrent;
        set => lvlCurrent = value;
    }

    public int LvlMax => lvlMax;
    public int ExpCurrent
    {
        get => expCurrent;
        set => expCurrent = value;
    }

    public int ExpToNextLvl => CalculateRequiredExp(lvlCurrent);
    public int ExpYield => baseExpOnDeath * LvlCurrent;
    
    public void AddExperience(int amount)
    {
        // Safety Check
        if (LvlCurrent >= lvlMax) return;
        if (amount < 0)
        {
            Debug.LogWarning("Cannot add negative experience. Use a separate method for draining EXP if needed.");
            return;
        }
        
        // Add the exp!
        expCurrent += amount;
        
        // Level up and Handle potential multiple level ups from a single large exp gain
        while (expCurrent >= ExpToNextLvl && lvlCurrent < lvlMax)
        {
            expCurrent -= ExpToNextLvl;
            lvlCurrent += 1;
            OnLevelUpdated?.Invoke(lvlCurrent);

            if (lvlCurrent >= lvlMax)
            {
                expCurrent = 0;
                OnMaxLevelReached?.Invoke();
                break;
            }
        }
        
        OnExperienceGained?.Invoke(expCurrent, ExpToNextLvl);
    }
    
    public int CalculateRequiredExp(int level)
    {
        if (lvlCurrent >= lvlMax) return 0;

        // Check if custom curve is enabled
        if (useCustomCurve && customExpCurve != null && customExpCurve.length > 0)
        {
            return Mathf.RoundToInt(customExpCurve.Evaluate(level)); 
        }
        
        // Else apply the default formula
        return Mathf.RoundToInt(baseExpRequirement * Mathf.Pow(level, expScalingFactor));
    }
    
    
    
}
