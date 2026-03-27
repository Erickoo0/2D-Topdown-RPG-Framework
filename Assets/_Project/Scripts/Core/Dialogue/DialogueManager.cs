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
    private GameObject[] _dialogueOptionButtons;
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
            _currentNode = body;
            _dialogueOptionButtons = new GameObject[body.dialogueOptions.Length];
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
        CreateOptions();
        
        // Pause the game
        PauseManager.SetPause(true);
    }
    
    private void CloseDialogue()
    {
        if (_dialogueOptionButtons != null)
        {
            foreach (var button in _dialogueOptionButtons)
            {
                Destroy(button);
            }
        }
        dialoguePanel.gameObject.SetActive(false);
        // Unpause the game
        PauseManager.SetPause(false);
    }

    private void CreateOptions()
    {
        if (_currentNode == null) return;
        if (_currentNode.dialogueOptions == null) return;
        int optionCount = _currentNode.dialogueOptions.Length;

        for (int i = 0; i < optionCount; i++)
        {
            var yOffset= -20 + (i * 20);
            string optionText = _currentNode.dialogueOptions[i].optionName;
            DialogueOption nextNode = _currentNode.dialogueOptions[i].nextNode;
            _dialogueOptionButtons[i] = Instantiate(dialogueOptionButton, dialoguePanel.transform.position + new Vector3(0, yOffset, 0), Quaternion.identity, dialoguePanel.transform);
            _dialogueOptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = optionText;
            //CreateOption(optionText, nextNode, yOffset);
        }
    }
    
    private void CreateOption(string optionText, DialogueOption nextNode, int yOffset)
    {
        GameObject optionButton = Instantiate(dialogueOptionButton, dialoguePanel.transform);
        optionButton.GetComponentInChildren<TextMeshProUGUI>().text = optionText;
    }
}
