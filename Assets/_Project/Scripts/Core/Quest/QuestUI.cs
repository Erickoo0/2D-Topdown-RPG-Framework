using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class QuestUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private GameObject questMenuPanel;
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
        
        // Tell the quest prefab to set up itself
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
    
    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!questMenuPanel.activeSelf)EventBus.RequestOpenMenu(questMenuPanel);
        else if (questMenuPanel.activeSelf) EventBus.RequestCloseMenu(questMenuPanel);
    }

}
