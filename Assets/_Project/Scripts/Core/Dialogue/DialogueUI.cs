using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{   
    [Header("Reference Data")] 
    [SerializeField] private Image dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueName;
    [SerializeField] private TypeWriter dialogueBody;
    [SerializeField] private Image dialoguePortrait;

    public bool IsTyping => dialogueBody.IsTyping;
    public bool IsVisible => dialoguePanel.gameObject.activeInHierarchy;
    
    public void ShowUI(bool active) => dialoguePanel.gameObject.SetActive(active);

    public void UpdateUI(string name, string line, Sprite portrait)
    {
        dialogueName.text = name;
        dialoguePortrait.sprite = portrait;
        dialogueBody.StartTyping(line);
    }

    public void FinishLineEarly()
    {
        dialogueBody.FinishInstantly();
    }
}
