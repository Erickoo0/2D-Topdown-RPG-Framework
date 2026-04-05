using UnityEngine;


[CreateAssetMenu(fileName = "FILENAME", menuName = "Quest/Quest Objective", order = 1)]
public class QuestObjective : ScriptableObject
{
    [SerializeField] private string objectiveTitle = "Objective Text";
    [SerializeField] private string targetID;
    [SerializeField] private int requiredAmount;
    
    public string ObjectiveTitle => objectiveTitle;
    public string TargetID => targetID;
    public int RequiredAmount => requiredAmount;
}
