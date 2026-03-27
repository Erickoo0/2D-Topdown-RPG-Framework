using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")] 
    [SerializeField] private string dialogueName;
    [SerializeField] private Sprite dialoguePortrait;
    [SerializeField] private DialogueNode dialogueStartNode;
    
    public bool CanInteract()
    {
        return true;
    }
    
    public void Interact()
    {
        if (!CanInteract()) return;
        DialogueManager.Instance.ControlDialogue(dialogueName, dialogueStartNode, dialoguePortrait);
    }
}