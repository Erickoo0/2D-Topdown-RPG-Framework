using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("Setup")] 
    [SerializeField] private GameObject levelBarPrefab;
    [SerializeField] private Vector3 offset = new Vector3(-1, 1f, 0);
    
    [Header("References")]
    [Tooltip("If blank, default to parent object")]
    [SerializeField] private Level lvlTarget;
    [SerializeField] private ProgressBar progressBarRef;
    [SerializeField] private TextMeshProUGUI lvlText;
    [SerializeField] private bool showLvlText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private bool showExpText;

    private void Awake()
    {
        if (lvlTarget == null) lvlTarget = GetComponentInParent<Level>();
    }

    private void OnEnable()
    {
        if (lvlTarget == null)
        {
            Debug.LogWarning("Level target is null");
            return;
        }
        
        lvlTarget.OnLevelUpdated += UpdateLvlUI;
        lvlTarget.OnExperienceGained += UpdateLvlUI;
        UpdateLvlUI();
    }

    private void OnDestroy()
    {
        if (lvlTarget == null) return;
        lvlTarget.OnLevelUpdated -= UpdateLvlUI;
        lvlTarget.OnExperienceGained -= UpdateLvlUI;
    }
    
    
    private void UpdateLvlUI()
    {
        // Update the Lvl text
        if (lvlText != null && showLvlText) 
            lvlText.text = $"Level: {lvlTarget.LvlCurrent}";
        
        if (progressBarRef != null)
        {        
            float expPercent = (float)lvlTarget.ExpCurrent / lvlTarget.ExpToNextLvl;
            progressBarRef.BarFill.fillAmount = expPercent;
            
            if (expText != null && showExpText)
                progressBarRef.BarText.text = $"Exp: {lvlTarget.ExpCurrent}/{lvlTarget.ExpToNextLvl}";

            string label = showExpText ? $"Exp: {lvlTarget.ExpCurrent}/{lvlTarget.ExpToNextLvl}" : "";
            progressBarRef.SetValues(lvlTarget.ExpCurrent, lvlTarget.ExpToNextLvl, label);
        }
    }
}
