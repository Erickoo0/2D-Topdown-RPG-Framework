using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Image barFill;
    [SerializeField] private TextMeshProUGUI barText;
    public Image BarFill => barFill;
    public TextMeshProUGUI BarText => barText;
    
    public void SetValues(float current, float max, string label = "")
    {
        if (barFill != null)
            barFill.fillAmount = Mathf.Clamp01(current / max);

        if (barText != null)
            barText.text = label;
    }
}
