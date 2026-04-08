using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("HP Settings")]
    public float hpMax = 100f;
    [SerializeField] private float hpCurrent;

    [Header("Entity References")] [Tooltip("The actual object the health component belongs to")] 
    [SerializeField] private GameObject entityRoot;

    [Header("Behavior Settings")] 
    [SerializeField] private bool destroyOnDeath = true;
    
    private bool _isDead = false;
    
    // Heal Overtime Variables
    private float _hpHealTimer;
    private float _hpHealTimerMax = 0.5f;
    private float _hpHealed;
    private float _hpHealedMax;
    private float _hpHealedPerTick;
    
    public event Action<float> OnHpUpdated;
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

            // If health hit zero
            if (hpCurrent <= 0 && !_isDead) SetDead();
        }
    }
    
    public bool HpIsHealingOverTime => _hpHealed < _hpHealedMax;

    private void Awake()
    {
        if (entityRoot == null) entityRoot = gameObject;
        if (hpCurrent <= 0) hpCurrent = hpMax;
    }
    
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

    private void SetDead()
    {
        _isDead = true;
        OnDeath?.Invoke();
        if (destroyOnDeath) Destroy(entityRoot);
    }
}