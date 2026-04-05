using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    // References
    private DialogueUI _dialogueUI;
    private DialogueOptionController _dialogueOptionController;
    
    private Npc _currentSpeaker;
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
        
        // Set the references
        _dialogueUI = GetComponent<DialogueUI>();
        _dialogueOptionController = GetComponent<DialogueOptionController>();
   }
    
    private void HandleInput()
    {
        // 1. If still typing, finish instantly
        if (_dialogueUI.IsTyping)
        {
            _dialogueUI.FinishLineEarly();
        }
        // 2. If waiting for option selection, do nothing
        else if (_isWaitingChoice)
        {
            return;
        }
        // 3. Continue
        else
        {
            ContinueDialogue();
        }
    }

    public void StartDialogue(Npc speaker)
    {
        // Guard Clause
        if (_dialogueUI.IsVisible) return;
        
        // Set the starting data
        _currentSpeaker = speaker;
        _currentNode = _currentSpeaker.DialogueStartNode;
        _currentLineIndex = 0;
        _isWaitingChoice = false;
        
        _dialogueOptionController.ClearOptions();
        
        EventBus.RequestOpenMenu(_dialogueUI.DialoguePanel);
        
        UpdateDisplay();
    }

    public void OnAdvanceDialogueInput(InputAction.CallbackContext context)
    {
        if (!context.performed || !_dialogueUI.IsVisible) return; 
        
        // 1. If still typing, finish instantly
        if (_dialogueUI.IsTyping)
        {
            _dialogueUI.FinishLineEarly();
        }
        // 2. If waiting for option selection, do nothing
        else if (_isWaitingChoice)
        {
            return;
        }
        // 3. Continue
        else
        {
            ContinueDialogue();
        }
    }

    private void ContinueDialogue()
    {
        // Advance the line index
        _currentLineIndex++;

        // Update the display to show new line index
        if (_currentLineIndex < _currentNode.dialogueLines.Length)
        {
            UpdateDisplay();
        }
        else
        {
            // Close the dialogue system if we have reached the end
            CloseDialogue();
        }
    }

    private void UpdateDisplay()
    {
        // Tell the Dialogue UI class to update the UI with the new line
        string currentLine = _currentNode.dialogueLines[_currentLineIndex];
        _dialogueUI.UpdateUI(_currentSpeaker.DialogueName, currentLine, _currentSpeaker.DialoguePortrait);
        
        CheckForOptions();
    }

    private void CheckForOptions()
    {
        // Check if dialogue is on the last line
        bool isLastLine = _currentLineIndex == _currentNode.dialogueLines.Length - 1;
        bool hasOptions = _currentNode.dialogueOptions != null && _currentNode.dialogueOptions.Length > 0;

        // Tell the Option Controller to create buttons if on the last line
        if (isLastLine && hasOptions)
        {
            _isWaitingChoice = true;
            _dialogueOptionController.CreateButtons(_currentNode.dialogueOptions, OnOptionSelected);        }
    }

    // Callback Function passed to the Buttons (activates on button click)
    public void OnOptionSelected(DialogueOption selectedOption)
    {
        // Tell the Option Controller to delete the options
        _isWaitingChoice = false;
        _dialogueOptionController.ClearOptions();

        // 1. Execut the options event if it has one
        if (!string.IsNullOrEmpty(selectedOption.dialogueEvent))
        {
            HandleDialogueEvents(selectedOption.dialogueEvent, selectedOption.eventData);
        }
        // 2. Advance to the next node if it has one
        if (selectedOption.nextNode != null) 
        {
            _currentNode = selectedOption.nextNode;
            _currentLineIndex = 0;
            UpdateDisplay();
        }
        else // 3. Close the dialogue if there is no next node
        {
            CloseDialogue();
        }
    }

    private void HandleDialogueEvents(string eventName, Object eventData = null)
    {
        if (string.IsNullOrEmpty(eventName)) return;

        switch (eventName)
        {
            case "OpenShop":
                EventBus.RequestDialogueEvent(eventName, _currentSpeaker.ShopList);
                break;
            
            case "AcceptQuest":
                EventBus.RequestDialogueEvent(eventName, eventData);
                break;
        }
    }
    
    private void CloseDialogue()
    {
        _dialogueOptionController.ClearOptions();
        _currentNode = null;
        
        EventBus.RequestCloseMenu(_dialogueUI.DialoguePanel);      
    }
}
