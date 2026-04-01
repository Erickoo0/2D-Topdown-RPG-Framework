using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image hpBarFill;
    [SerializeField] private TextMeshProUGUI hpText;
    //[SerializeField] private Text healthText;
    private Health health;

    private void Start()
    {
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        health.OnHpUpdated += UpdateHealthUI;
        UpdateHealthUI(health.HpCurrent);
    }

    private void OnDestroy()
    {
        health.OnHpUpdated -= UpdateHealthUI;
    }

    private void UpdateHealthUI(float hpCurrent)
    {
        float hpPercent = hpCurrent / health.hpMax;
        hpBarFill.fillAmount = hpPercent;
        hpText.text = ($"Hp: {hpCurrent}/{health.hpMax}");
    }
}