using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("HP Settings")]
    public float hpMax = 100f;
    [SerializeField] private float hpCurrent;
    
    // Heal Overtime Variables
    private float _hpHealTimer;
    private float _hpHealTimerMax = 0.5f;
    private float _hpHealed;
    private float _hpHealedMax;
    private float _hpHealedPerTick;
    
    // Fired whenever health changes, so UI or other systems can refresh.
    public event Action<float> OnHpUpdated;
    // Fired when health reaches 0.
    public event Action OnDeath;
    
    // Health Property
    public float HpCurrent
    {
        get => hpCurrent;
        set
        {
            float hpPrevious = hpCurrent;

            // Clamp health so it never goes below 0 or above max.
            hpCurrent = Mathf.Clamp(value, 0, hpMax);

            // Only notify listeners if health actually changed.
            if (!Mathf.Approximately(hpCurrent, hpPrevious))
            {
                float difference = hpCurrent - hpPrevious;
                int differenceRounded = Mathf.RoundToInt(difference);
                EventBus.RequestFloatingText(differenceRounded, transform.position);
                
                OnHpUpdated?.Invoke(hpCurrent);
            }

            // If health hit zero, notify death listeners.
            if (hpCurrent <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
    
    public bool HpIsHealingOverTime => _hpHealed < _hpHealedMax;
    
    private void Update()
    {
        // Exit early if not healing
        if (!HpIsHealingOverTime)
        {
            // Reset variables just to be safe when not active
            _hpHealedMax = 0;
            _hpHealed = 0;
            _hpHealTimer = 0;
            return;
        }
        
        // Begin Heal overtime logic
        _hpHealTimer += Time.deltaTime;
        
        // Heal & reset timer
        if (_hpHealTimer >= _hpHealTimerMax)
        {
            HpCurrent += _hpHealedPerTick;
            _hpHealed += _hpHealedPerTick;
            
            _hpHealTimer -= _hpHealTimerMax;
        }
    }
    
    public void HpHealInstant(float hpHealAmount)
    {
        HpCurrent += hpHealAmount;
    }
    
    public void HpHealOverTime(float hpHealAmount, float hpHealDuration)
    {
        _hpHealedPerTick = (hpHealAmount / hpHealDuration) * _hpHealTimerMax;
        _hpHealedMax = hpHealAmount;
        _hpHealed = 0;
    }
}