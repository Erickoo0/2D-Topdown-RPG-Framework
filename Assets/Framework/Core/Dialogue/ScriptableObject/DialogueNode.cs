using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue System/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public string[] dialogueLines;
    public DialogueOption[] dialogueOptions;
    public string nodeEvent; // Future, if we want to play animations or events when the a specific dialogue node starts
}

[System.Serializable]
public class DialogueOption
{
    public string optionName;
    public DialogueNode nextNode;
    
    [Header("Event Data")]
    public string dialogueEvent;
    public string eventParameter; // For quests, put the QuestID here.
}