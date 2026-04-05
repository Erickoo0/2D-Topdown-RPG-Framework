using System;
using UnityEngine;

public static class EventBus
{
    //-----------------------Dialogue Events--------------------------
    //Signals when a dialogue option is selected that has an action
    public static Action<string, object> OnDialogueEventRequested;
    public static void RequestDialogueEvent(string dialogueEvent, object data)
    {
        OnDialogueEventRequested?.Invoke(dialogueEvent, data);
    }
    
    //-----------------------UI Events--------------------------
    public static event Action<GameObject> OnMenuOpenRequested;
    public static void RequestOpenMenu(GameObject menu) => OnMenuOpenRequested?.Invoke(menu);

    public static event Action<GameObject> OnMenuCloseRequested;
    public static void RequestCloseMenu(GameObject menu = null) => OnMenuCloseRequested?.Invoke(menu);
    
    //--------------------------Quest Events-------------------------
    public static event Action<string, int> OnUpdateQuestObjectiveRequested;
    public static void RequestUpdateQuestObjective(string targetID, int number) => OnUpdateQuestObjectiveRequested?.Invoke(targetID, number);
    
    public static event Action OnUpdateQuestRequested;
    public static void RequestUpdateQuest()=> OnUpdateQuestRequested?.Invoke();
    
    
    
    //--------------------------Combat Events-------------------------
    // Signals when a floating text gets requested
    public static event Action<int, Vector3> OnFloatingTextRequested;
    // Any script can call this method to request a floating number
    public static void RequestFloatingText(int amount, Vector3 position)
    {
        OnFloatingTextRequested?.Invoke(amount, position);
    }
}