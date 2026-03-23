using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")] 
    [SerializeField] private string dialogueName;
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private Sprite dialoguePortrait;


    public bool CanInteract()
    {
        return true;
    }
    
    public void Interact()
    {
        if (!CanInteract()) return;
        DialogueManager.Instance.ControlDialogue(dialogueName, dialogueLines, dialoguePortrait);
    }


}