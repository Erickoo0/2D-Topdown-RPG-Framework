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
    private int _currentLineIndex;
    private bool _isWaitingChoice = false;

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
            // 1. If typewriter is still typing, finish it instantly (even if options are on screen)
            if (dialogueBody.IsTyping) dialogueBody.FinishInstantly();
            
            // 2. If typing is finished, check if we are waiting for a choice, do nothing
            else if (_isWaitingChoice) return;
            
            // 3. Otherwise, go to the next line / close the dialogue
            else ContinueDialogue();
        }
    }
    
    private void SetDialogue(string name, DialogueNode body, Sprite portrait)
    {
        // Clear leftover buttons if they exist
        DestroyOptions();
        
        //Set the Node
        _currentNode = body;
        _currentLineIndex = 0;
        _isWaitingChoice = false;
        
        // Set the name, portrait, and index
        dialogueName.text = name;
        dialoguePortrait.sprite = portrait;
        
        // Set the options
        if (_currentNode.dialogueOptions != null) _dialogueOptionButtons = new GameObject[body.dialogueOptions.Length];
        
        // Set the body
        dialoguePanel.gameObject.SetActive(true);
        
        // Pause the game
        PauseManager.SetPause(true);
        
        // Play Line and Check Options
        PlayLineAndCheckOptions();
    }
    
    private void CloseDialogue()
    {
        DestroyOptions();
        dialoguePanel.gameObject.SetActive(false);
        // Unpause the game
        PauseManager.SetPause(false);
    }

    private void ContinueDialogue()
    {
        _currentLineIndex++;
        
        // 1. Are there still lines left to continue?
        if (_currentLineIndex < _currentNode.dialogueLines.Length)  PlayLineAndCheckOptions();
            else CloseDialogue();
    }

    private void PlayLineAndCheckOptions()
    {
        // 1. Start the Typewriter
        dialogueBody.StartTyping(_currentNode.dialogueLines[_currentLineIndex]);
        
        // 2. Is this the LAST line in the array?
        if (_currentLineIndex == _currentNode.dialogueLines.Length - 1)
        {
            // 3. Does this node have options to display?
            if (_currentNode.dialogueOptions != null && _currentNode.dialogueOptions.Length > 0)
            {
                _isWaitingChoice = true;
                CreateOptions();
            }
        }
    }

    private void CreateOptions()
    {
        if (_currentNode == null) return;
        if (_currentNode.dialogueOptions == null) return;
        
        for (int i = 0; i < _currentNode.dialogueOptions.Length; i++)
        {
            // Capture the index for the listener (C# for loop quirk with using i directly in delegates)
            int index = i;
            DialogueNode targetNode = _currentNode.dialogueOptions[index].nextNode;
            
            var yOffset= 270 + (i * 110);
            var xOffset = 590;
            
            // Pass the NPC name and the index of the option to the listener
            string optionText = _currentNode.dialogueOptions[i].optionName;
            
            // Create the button
            _dialogueOptionButtons[i] = Instantiate(dialogueOptionButton, dialoguePanel.transform.position + new Vector3(xOffset, yOffset, 0), Quaternion.identity, dialoguePanel.transform);
            
            // Get the TextMeshProUGUI component in the button prefab and set the text
            _dialogueOptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = optionText;
            
            // Get the Button component and add a listener to it
            _dialogueOptionButtons[i].GetComponent<Button>().onClick.AddListener(() => OnOptionSelected(targetNode));
        }
    }

    private void DestroyOptions()
    {
        if (_dialogueOptionButtons != null)
        {
            foreach (var button in _dialogueOptionButtons)
            {
                Destroy(button);
            }
        }
    }

    public void OnOptionSelected(DialogueNode nextNode)
    {
        if (nextNode != null)
        {
            SetDialogue(dialogueName.text, nextNode, dialoguePortrait.sprite);
        }
        else
        {
            // Execute Code Method here
            if (_currentNode.dialogueEvent != null) EventBus.RequestDialogueEvent( _currentNode.dialogueEvent);
            CloseDialogue();
        }
    }
}
