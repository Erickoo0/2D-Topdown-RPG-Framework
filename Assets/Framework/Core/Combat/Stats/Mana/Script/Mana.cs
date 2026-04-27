using UnityEngine;
using System;

public class Mana : MonoBehaviour
{
    [Header("MP Settings")]
    [SerializeField] private float mpBase = 100f;
    public float mpMax;
    private float _mpCurrent;
    [SerializeField] private float mpPerLvl = 10f;
    
    [Header("References")] 
    [Tooltip("The actual object the health component belongs to")] 
    [SerializeField] private GameObject entityRoot;
    private Level _lvlComponent;
    
    // Heal Overtime Variables
    private float _mpHealTimer;
    private float _mpHealTimerMax = 0.5f;
    private float _mpHealed;
    private float _mpHealedMax;
    private float _mpHealedPerTick;
    
    public event Action OnMpUpdated;
    
    // Health Property
    public float MpCurrent
    {
        get => _mpCurrent;
        set
        {
            float mpPrevious = _mpCurrent;

            // Clamp health so it never goes below 0 or above max.
            _mpCurrent = Mathf.Clamp(value, 0, mpMax);

            // Only notify listeners if health actually changed.
            if (!Mathf.Approximately(_mpCurrent, mpPrevious))
            {
                float difference = _mpCurrent - mpPrevious;
                int differenceRounded = Mathf.RoundToInt(difference);
                EventBus.RequestFloatingText(differenceRounded, transform.position);
                
                OnMpUpdated?.Invoke();
            }
        }
    }
    public bool MpIsHealingOverTime => _mpHealed < _mpHealedMax;
    
    private void Awake()
    {
        if (entityRoot == null) entityRoot = gameObject;
        
        _lvlComponent = GetComponentInParent<Level>();
        if (_lvlComponent != null)
        {
            UpdateMpOnLevelUp();
        }
    }
    
    private void OnEnable() => _lvlComponent.OnLevelUpdated += UpdateMpOnLevelUp;
    
    private void OnDisable() => _lvlComponent.OnLevelUpdated -= UpdateMpOnLevelUp;
    
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
    
    private void UpdateMpOnLevelUp()
    {
        mpMax = mpBase + (_lvlComponent.LvlCurrent - 1) * mpPerLvl;
        _mpCurrent = mpMax;
        OnMpUpdated?.Invoke();
    }
}
