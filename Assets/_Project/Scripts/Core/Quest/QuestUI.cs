using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private Image questMenuPanel;
    [SerializeField] private GameObject questSoMenuPanel;
    [SerializeField] private GameObject questMenuPrefab;
    [SerializeField] private Image questPanel;
    [SerializeField] private GameObject questSoPanel;
    [SerializeField] private GameObject questPrefab;

    private List<Quest> _activeQuestsListUI = new List<Quest>();

    public void AddQuestUI(QuestActive quest)
    {
        // Spawn the quest Prefab
        GameObject newQuest = Instantiate(questPrefab, questSoPanel.transform);
        
        // Tell the quest prefab to setup itself
        newQuest.GetComponent<Quest>().Setup(quest);
        
        // Add the quest prefab to the Quest UI List
        _activeQuestsListUI.Add(newQuest.GetComponent<Quest>());
    }

    public void UpdateQuestUI(QuestActive updatedQuest)
    {
        // Loop through all quests in UI list 
        foreach (Quest quest in _activeQuestsListUI)
        {
            // If the quest ID matches, update the quest
            if (quest.QuestID == updatedQuest.QuestData.QuestID)
            {
                quest.UpdateProgressText(updatedQuest);
            }
        }
    }

}
