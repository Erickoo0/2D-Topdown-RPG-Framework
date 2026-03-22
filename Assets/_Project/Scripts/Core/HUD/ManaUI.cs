using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private Image mpBarFill;
    [SerializeField] TextMeshProUGUI mpText;
    private Mana mana;

    private void Start()
    {
        mana = GameObject.FindGameObjectWithTag("Player").GetComponent<Mana>();
        mana.OnMpUpdated += UpdateManaUI;
        UpdateManaUI(mana.MpCurrent);
    }

    private void OnDestroy()
    {
        mana.OnMpUpdated -= UpdateManaUI;
    }

    private void UpdateManaUI(float mpCurrent)
    {
        float mpPercent = mpCurrent / mana.mpMax;
        mpBarFill.fillAmount = mpPercent;
        mpText.text = ($"Mp: {mpCurrent}/{mana.mpMax}");
    }
}