using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")] // The only purpose of this script is to hold reference to the fill image that the HealthUI component can find
    [SerializeField] private Image hpBarFill;

    [SerializeField] private TextMeshProUGUI healthText;
    public Image HpBarFill => hpBarFill;
    public TextMeshProUGUI HealthText => healthText;
    
    
}
