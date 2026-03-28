using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue System/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public string[] dialogueLines;
    public DialogueOption[] dialogueOptions;
    public string dialogueEvent;
}

[System.Serializable]
public class DialogueOption
{
    public string optionName;
    public DialogueNode nextNode;
}