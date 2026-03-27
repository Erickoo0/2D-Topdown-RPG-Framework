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

    [Header("Dialogue UI Prefabs")] 
    [SerializeField] private GameObject dialogueOptionButton;
    private DialogueNode _currentNode;

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
    
    public void ControlDialogue(string name, DialogueNode body, Sprite portrait)
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
            else CloseDialogue();
        }

    }
    
    private void SetDialogue(string name, DialogueNode body, Sprite portrait)
    {
        // Set the name, portrait, and index
        dialogueName.text = name;
        dialoguePortrait.sprite = portrait;
        
        // Set the body
        dialoguePanel.gameObject.SetActive(true);
        dialogueBody.StartTyping(body);
        
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
