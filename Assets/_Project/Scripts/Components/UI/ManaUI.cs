using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Mana manaUI;
    [SerializeField] private Image mpBarFill;
    [SerializeField] TextMeshProUGUI mpText;


    private void Start()
    {
        manaUI.OnMpUpdated += UpdateManaUI;
        UpdateManaUI(manaUI.MpCurrent);
    }

    private void OnDestroy()
    {
        manaUI.OnMpUpdated -= UpdateManaUI;
    }

    private void UpdateManaUI(float mpCurrent)
    {
        float mpPercent = mpCurrent / manaUI.mpMax;
        mpBarFill.fillAmount = mpPercent;
        mpText.text = ($"Mp: {mpCurrent}/{manaUI.mpMax}");
    }
}