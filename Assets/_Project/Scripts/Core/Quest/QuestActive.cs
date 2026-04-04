using UnityEngine;

[System.Serializable]
public class QuestActive
{
    public QuestSo QuestData { get; private set; }
    public int[] ObjectiveProgress { get; private set; } 
    public bool IsCompleted { get; private set; }

    // Constructor that setups the quest
    public QuestActive(QuestSo questData)
    {
        QuestData = questData;
        // Initialize the objective progress array with the required amount for each objective
        ObjectiveProgress = new int[QuestData.QuestObjectives.Count];
        IsCompleted = false;
    }
    
    public void CheckQuestCompletion()
    {
        if (IsCompleted) return; // Skip if the quest is already completed

        int completedCount = 0; // Counts how many objectives are completed

        // Check for objective completion 
        for (int i = 0; i < QuestData.QuestObjectives.Count; i++)
        {
            if (ObjectiveProgress[i] >= QuestData.QuestObjectives[i].RequiredAmount)
            {
                completedCount++;
                Debug.Log($"Quest {QuestData.QuestName}: Objective {QuestData.QuestObjectives[i].ObjectiveTitle}: completed!");
            }
        }

        // If all objectives are completed, mark the quest as completed
        if (completedCount >= QuestData.QuestObjectives.Count)
        {
            IsCompleted = true;
            Debug.Log($"Quest {QuestData.QuestName}: Completed!");
        }
    }

    // Method to add progress to an objective (called by Source -> EventBus -> QuestManager)
    public void AddObjectiveProgress(int objectiveIndex, int progress)
    {
        ObjectiveProgress[objectiveIndex] += progress;
        CheckQuestCompletion();
    }
}
