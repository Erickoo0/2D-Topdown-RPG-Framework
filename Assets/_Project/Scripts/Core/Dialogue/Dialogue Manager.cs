using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("Reference Data")] 
    [SerializeField] private Image dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueName;
    [SerializeField] private TextMeshProUGUI dialogueBody;
    [SerializeField] private Image dialoguePortrait;

    //private int inputTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple DialogueManagers detected. Disabling script.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void ControlDialogue(string name, string body, Sprite portrait)
    {
        // Enable the dialogue panel if it's not already active'
        if (!dialoguePanel.gameObject.activeSelf)
        {
            SetDialogue(name, body, portrait);
        }
        // Disable the dialogue panel if it's already active'
        else CloseDialogue();

    }
    
    private void SetDialogue(string name, string body, Sprite portrait)
    {
        dialogueName.text = name;
        dialogueBody.text = body;
        dialoguePortrait.sprite = portrait;
        dialoguePanel.gameObject.SetActive(true);
        
        // Pause the game
        PauseManager.SetPause(true);
    }
    
    private void CloseDialogue()
    {
        dialoguePanel.gameObject.SetActive(false);
        
        // Unpause the game
        PauseManager.SetPause(false);
    }
}
