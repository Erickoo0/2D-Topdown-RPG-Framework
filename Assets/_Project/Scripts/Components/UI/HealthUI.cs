using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Setup")] 
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);
    
    [Header("UI")]
    [Tooltip("If blank, default to parent object")]
    [SerializeField] private Health healthTarget;
    [SerializeField] private HealthBar healthBarRef;
    [SerializeField] private bool showText;


    private void Awake()
    {
        // If no health target (assigned ui), assume it's the parent object.
        if (healthTarget == null) healthTarget = GetComponentInParent<Health>();
        if (healthBarRef == null && healthBarPrefab != null) SpawnHealthBar();
    }

    private void Start()
    {
        if (healthTarget == null)
        {
            Debug.LogWarning("Health target is null");
            return;
        }
        
        healthTarget.OnHpUpdated += UpdateHealthUI;
        UpdateHealthUI(healthTarget.HpCurrent);
    }

    private void OnDestroy()
    {
        if (healthTarget == null) return;
        
        healthTarget.OnHpUpdated -= UpdateHealthUI;
    }

    private void SpawnHealthBar()
    {
        // Create the health bar as a child of this gameobject
        GameObject healthbarInstance = Instantiate(healthBarPrefab, transform);
        healthbarInstance.transform.localPosition = offset;
        
        // Assign the components from the prefab
        healthBarRef = healthbarInstance.GetComponent<HealthBar>();
        if (healthBarRef == null) Debug.LogWarning("Health bar instance is null");
    }
    
    private void UpdateHealthUI(float hpCurrent)
    {
        // Safety Check
        if (healthTarget is null || healthBarRef is null) return;
        
        float hpPercent = hpCurrent / healthTarget.hpMax;
        
        if (healthBarRef.HpBarFill is not null) healthBarRef.HpBarFill.fillAmount = hpPercent;
        if (healthBarRef.HealthText is not null && showText) healthBarRef.HealthText.text = ($"Hp: {hpCurrent}/{healthTarget.hpMax}");
        else if (!showText) healthBarRef.HealthText!.text = "";
    }
}