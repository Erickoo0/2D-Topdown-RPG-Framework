using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{
    [Header("Component References")] 
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questProgress;
    
    public string QuestID { get; private set;}
    
    public string QuestName => questName.text;
    public string QuestDescription => questDescription.text;
    public string QuestProgress => questProgress.text;

    public void Setup(QuestActive quest)
    {
        QuestID = quest.QuestData.QuestID;
        questName.text = quest.QuestData.QuestName;
        questDescription.text = quest.QuestData.QuestDescription;

        UpdateProgressText(quest);
    }

    public void UpdateProgressText(QuestActive quest)
    {
        // Clear the text
        questProgress.text = "";
        
        // Loop through all objectives and write them out
        for (int i = 0; i < quest.QuestData.QuestObjectives.Count; i++)
        {
            QuestObjective objectiveData = quest.QuestData.QuestObjectives[i];
            int currentProgress = quest.ObjectiveProgress[i];
            int requiredAmount = objectiveData.RequiredAmount;
            
            questProgress.text = $"- {objectiveData.ObjectiveTitle}: {currentProgress} / {requiredAmount}";
        }
        
        if (quest.IsCompleted)
        {
            questProgress.text = "<color=green>Ready to Turn In!</color>";
        }
    }
}
