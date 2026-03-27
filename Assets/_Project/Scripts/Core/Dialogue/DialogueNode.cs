using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue System/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public string text;
    public DialogueOption[] dialogueOptions;
    //public UnityEvent onNodeReached;
}

[System.Serializable]
public class DialogueOption
{
    public string optionName;
    public string text;
    public DialogueOption nextNode;
}