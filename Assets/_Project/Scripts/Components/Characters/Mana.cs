using UnityEngine;
using System;

public class Mana : MonoBehaviour
{
    [Header("MP Settings")]
    public float mpMax = 100f;
    [SerializeField] private float mpCurrent;
    
    // Heal Overtime Variables
    private float _mpHealTimer;
    private float _mpHealTimerMax = 0.15f;
    private float _mpHealed;
    private float _mpHealedMax;
    private float _mpHealedPerTick;
    
    public event Action<float> OnMpUpdated;
    
    // Left exactly as requested
    // Health Property
    public float MpCurrent
    {
        get => mpCurrent;
        set
        {
            float mpPrevious = mpCurrent;

            // Clamp health so it never goes below 0 or above max.
            mpCurrent = Mathf.Clamp(value, 0, mpMax);

            // Only notify listeners if health actually changed.
            if (mpCurrent != mpPrevious)
            {
                float difference = mpCurrent - mpPrevious;
                int differenceRounded = Mathf.RoundToInt(difference);
                CombatEvents.RequestFloatingText(differenceRounded, transform.position);
                OnMpUpdated?.Invoke(mpCurrent);
            }
        }
    }
    public bool MpIsHealingOverTime => _mpHealed < _mpHealedMax;
    
    private void Update()
    {
        // Exit early if not healing
        if (!MpIsHealingOverTime)
        {
            // Reset variables just to be safe when not active
            _mpHealedMax = 0;
            _mpHealed = 0;
            _mpHealTimer = 0;
            return;
        }
        
        // Begin Heal overtime logic
        _mpHealTimer += Time.deltaTime;
        
        // Heal & reset timer
        if (_mpHealTimer >= _mpHealTimerMax)
        {
            MpCurrent += _mpHealedPerTick;
            _mpHealed += _mpHealedPerTick;
            
            _mpHealTimer -= _mpHealTimerMax;
        }
    }
    
    public void MpHealInstant(float mpHealAmount)
    {
        MpCurrent += mpHealAmount;
    }
    
    public void MpHealOverTime(float mpHealAmount, float mpHealDuration)
    {
        _mpHealedPerTick = (mpHealAmount / mpHealDuration) * _mpHealTimerMax;
        _mpHealedMax = mpHealAmount;
        _mpHealed = 0;
    }
}
