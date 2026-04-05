using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FILENAME", menuName = "Quest/Quest", order = 0)]
public class QuestSo : ScriptableObject
{
    [SerializeField] private string questID;
    [SerializeField] private string questName;
    [SerializeField] private string questDescription;
    [SerializeField] private List<QuestObjective> questObjectives;
    
    public string QuestID => questID;
    public string QuestName => questName;
    public string QuestDescription => questDescription;
    public List<QuestObjective> QuestObjectives => questObjectives;
}
