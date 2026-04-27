using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("If blank, default to parent object")]
    [SerializeField] private Mana mpComponent;
    [SerializeField] private ProgressBar progressBarRef;
    [SerializeField] private bool showText;


    private void Awake()
    {
        if (mpComponent == null) mpComponent = GetComponentInParent<Mana>();
    }
    
    private void OnEnable()
    {
        if (mpComponent == null)
        {
            Debug.LogWarning("Health target is null");
            return;
        }
        
        mpComponent.OnMpUpdated += UpdateMpUI;
        UpdateMpUI();
    }
    
    private void OnDisable() => mpComponent.OnMpUpdated -= UpdateMpUI;
    

    private void UpdateMpUI()
    {
        // Safety Check 
        if (mpComponent is null || progressBarRef is null) return;

        string label = showText ? $"MP: {mpComponent.MpCurrent}/{mpComponent.mpMax}" : "";
        progressBarRef.SetValues(mpComponent.MpCurrent, mpComponent.mpMax, label);
    }
}