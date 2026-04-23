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
    [SerializeField] private Health hpTarget;
    [SerializeField] private ProgressBar progressBarRef;
    [SerializeField] private bool showText;


    private void Awake()
    {
        // If no health target (assigned ui), assume it's the parent object.
        if (hpTarget == null) hpTarget = GetComponentInParent<Health>();
        if (progressBarRef == null && healthBarPrefab != null) SpawnHealthBar();
    }

    private void Start()
    {
        if (hpTarget == null)
        {
            Debug.LogWarning("Health target is null");
            return;
        }
        
        hpTarget.OnHpUpdated += UpdateHpUI;
        UpdateHpUI(hpTarget.HpCurrent);
    }

    private void OnDestroy()
    {
        if (hpTarget == null) return;
        
        hpTarget.OnHpUpdated -= UpdateHpUI;
    }

    private void SpawnHealthBar()
    {
        // Create the health bar as a child of this gameobject
        GameObject healthbarInstance = Instantiate(healthBarPrefab, transform);
        healthbarInstance.transform.localPosition = offset;
        
        // Assign the components from the prefab
        progressBarRef = healthbarInstance.GetComponent<ProgressBar>();
        if (progressBarRef == null) Debug.LogWarning("Health bar instance is null");
    }
    
    private void UpdateHpUI(float hpCurrent)
    {
        // Safety Check
        if (hpTarget is null || progressBarRef is null) return;
        
        string label = showText ? $"HP: {hpCurrent}/{hpTarget.hpMax}" : "";
        progressBarRef.SetValues(hpCurrent, hpTarget.hpMax, label);
    }
}