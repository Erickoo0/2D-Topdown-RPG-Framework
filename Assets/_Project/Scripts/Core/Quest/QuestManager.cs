using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private QuestUI questUI;
    
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

    private void OnEnable()
    {
        EventBus.OnUpdateQuestObjectiveRequested += HandleObjectiveUpdate;
        EventBus.OnDialogueEventRequested += AcceptQuest;
    }

    private void OnDisable()
    {
        EventBus.OnUpdateQuestObjectiveRequested -= HandleObjectiveUpdate;
        EventBus.OnDialogueEventRequested -= AcceptQuest;
    }

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
                    questUI.UpdateQuestUI(questActive);
                    Debug.Log($"Quest {questActive.QuestData.QuestName} Objective {i} updated by {amount}.");
                }
            }
        }
    }


    
    public void AcceptQuest(string dialogueEvent,object questData)
    {
        // Safety Check
        if (dialogueEvent != "AcceptQuest") return;
        if (questData == null) return;
        
        // Cast the object to QuestSo
        QuestSo questDataSo = questData as QuestSo; // Cast the object to QuestSo
        if (questDataSo == null) return;
        
        // Check if we already have the quest to avoid duplicates
        if (_questList.Exists(q => q.QuestData.QuestID == questDataSo.QuestID)) return;
        
        // Create a new QuestActive object and add it to the list
        _questList.Add(new QuestActive(questDataSo));
        Debug.Log($"Quest {questDataSo.QuestName} accepted.");
        
        // Tell the QuestUI to Update
        var index = _questList.Count-1; // Get the index of the last quest (To pass the QuestActive object to QuestUI)
        questUI.AddQuestUI(_questList[index]);   
    }

}
