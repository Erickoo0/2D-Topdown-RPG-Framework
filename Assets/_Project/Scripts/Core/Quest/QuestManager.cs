using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        private List<QuestActive> _questList = new List<QuestActive>();
        
        public List<QuestActive> QuestList => _questList;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.unityLogger.Log("Multiple QuestManagers detected. Disabling script.");
                return;
            }

            Instance = this;
        }

        private void OnEnable() => EventBus.OnUpdateQuestObjectiveRequested += HandleObjectiveUpdate;

        private void OnDisable() => EventBus.OnUpdateQuestObjectiveRequested -= HandleObjectiveUpdate;

        private void HandleObjectiveUpdate(string targetID, int amount)
        {
            foreach (QuestActive questActive in _questList)
            {
                if (questActive.IsCompleted) continue; // Skip the quest if its already completed
                
                // Look through every objective in the quest, and check if Event target ID matches the objective target ID
                for (int i = 0; i < questActive.QuestData.QuestObjectives.Count; i++)
                {
                    
                    // If it does, add the amount to the objective progress
                    if (questActive.QuestData.QuestObjectives[i].TargetID == targetID)
                    {
                        questActive.AddObjectiveProgress(i, amount);
                        Debug.Log($"Quest {questActive.QuestData.QuestName} Objective {i} updated by {amount}.");
                    }
                }
            }
        }
        
        public void AcceptQuest(QuestSo questData)
        {
            // Safety Check
            if (questData == null) return;
            // Check if we already have the quest to avoid duplicates
            if (_questList.Exists(q => q.QuestData.QuestID == questData.QuestID)) return;
            
            _questList.Add(new QuestActive(questData));
            Debug.Log($"Quest {questData.QuestName} accepted.");
        }

    }
}