using System;
using UnityEngine;

public static class EventBus
{
    //-----------------------Dialogue Events--------------------------
    //Signals when a dialogue option is selected that has an action
    public static Action<string> OnDialogueEventRequested;
    public static void RequestDialogueEvent(string dialogueEvent)
    {
        OnDialogueEventRequested?.Invoke(dialogueEvent);
    }
    
    
    
    //--------------------------Combat Events-------------------------
    // Signals when a floating text gets requested
    public static event Action<int, Vector3> OnFloatingTextRequested;
    // Any script can call this method to request a floating number
    public static void RequestFloatingText(int amount, Vector3 position)
    {
        OnFloatingTextRequested?.Invoke(amount, position);
    }
}