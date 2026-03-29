using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")] 
    [SerializeField] private string dialogueName;
    [SerializeField] private Sprite dialoguePortrait;
    [SerializeField] private DialogueNode dialogueStartNode;

    [Header("Shop Data")] 
    [SerializeField] private ItemData[] shopList;
    
    public ItemData[] ShopList => shopList;
    
    // Properties so DialogueManager can read the private variables
    public string  DialogueName => dialogueName;
    public Sprite DialoguePortrait => dialoguePortrait;
    public DialogueNode DialogueStartNode => dialogueStartNode;
    
    public bool CanInteract()
    {
        return true;
    }
    
    public void Interact()
    {
        if (!CanInteract()) return;
        DialogueManager.Instance.ControlDialogue(this);
    }
}