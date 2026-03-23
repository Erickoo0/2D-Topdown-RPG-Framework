using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("Reference Data")] 
    [SerializeField] private Image dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueName;
    [SerializeField] private TypeWriter dialogueBody;
    [SerializeField] private Image dialoguePortrait;

    private string[] dialogueLines;
    private int dialogueLineIndex;

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
    
    public void ControlDialogue(string name, string[] body, Sprite portrait)
    {
        // Enable the dialogue panel if it's not already active'
        if (!dialoguePanel.gameObject.activeSelf)
        {
            SetDialogue(name, body, portrait);
        }
        else
        {
            // 1. If typewriter is still typing, finish it instantly
            if (dialogueBody.IsTyping) dialogueBody.FinishInstantly();
            // 2. If typewriter is done typing, move to the next line
            else if (dialogueLineIndex < dialogueLines.Length - 1)
            {
                dialogueLineIndex++;
                dialogueBody.StartTyping(dialogueLines[dialogueLineIndex]);
            }
            // 3. If no more lines, close the dialogue
            else
            {
                CloseDialogue();
            }
        }

    }
    
    private void SetDialogue(string name, string[] body, Sprite portrait)
    {
        // Set the name, portrait, and index
        dialogueName.text = name;
        dialoguePortrait.sprite = portrait;
        dialogueLineIndex = 0;
        
        // Set the body
        dialogueLines = body;
        dialoguePanel.gameObject.SetActive(true);
        dialogueBody.StartTyping(dialogueLines[dialogueLineIndex]);
        
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
