using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private float hpBase = 100f;
    public float hpMax;
    private float _hpCurrent;
    [SerializeField] private float hpPerLvl = 10f;

    [Header("References")] 
    [Tooltip("The actual object the health component belongs to")] 
    [SerializeField] private GameObject entityRoot;
    private Level _lvlComponent;

    [Header("Behavior Settings")] 
    [SerializeField] private bool destroyOnDeath = true;
    
    private bool _isDead = false;
    
    // Heal Overtime Variables
    private float _hpHealTimer;
    private float _hpHealTimerMax = 0.5f;
    private float _hpHealed;
    private float _hpHealedMax;
    private float _hpHealedPerTick;
    
    public event Action OnHpUpdated;
    public event Action OnDeath;
    
    // Health Property
    public float HpCurrent
    {
        get => _hpCurrent;
        set
        {
            float hpPrevious = _hpCurrent;

            // Clamp health so it never goes below 0 or above max.
            _hpCurrent = Mathf.Clamp(value, 0, hpMax);

            // Only notify listeners if health actually changed.
            if (!Mathf.Approximately(_hpCurrent, hpPrevious))
            {
                float difference = _hpCurrent - hpPrevious;
                int differenceRounded = Mathf.RoundToInt(difference);
                EventBus.RequestFloatingText(differenceRounded, transform.position);
                
                OnHpUpdated?.Invoke();
            }

            // If health hit zero
            if (_hpCurrent <= 0 && !_isDead) SetDead();
        }
    }
    
    public bool HpIsHealingOverTime => _hpHealed < _hpHealedMax;

    private void Awake()
    {
        if (entityRoot == null) entityRoot = gameObject;
        
        _lvlComponent = GetComponentInParent<Level>();
        if (_lvlComponent != null)
        {
            UpdateHpOnLevelUp();
        }
    }

    private void OnEnable() => _lvlComponent.OnLevelUpdated += UpdateHpOnLevelUp;
    
    private void OnDisable() => _lvlComponent.OnLevelUpdated -= UpdateHpOnLevelUp;
    
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
        OnDeath?.Invoke(); // Event for LOCAL systems like ItemContainers
        EventBus.RequestEntityDeathUpdate(entityRoot); // Alert the EventBus of entity death
        if (destroyOnDeath) Destroy(entityRoot);
    }
    
    private void UpdateHpOnLevelUp()
    {
        hpMax = hpBase + (_lvlComponent.LvlCurrent - 1) * hpPerLvl;
        _hpCurrent = hpMax;
        OnHpUpdated?.Invoke();
    }
}