using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds information of all Saved Data
/// </summary>
[System.Serializable]
public class SaveData
{
    //----Player Variables----
    public Vector3 playerPosition;
    public float maxHealth;
    public float currentHealth;
    public float maxMana;
    public float currentMana;
    public int currentLevel;
    public int currentExperience;
    
    //----Location Variables----
    public string currentLocation;
    
    //----Inventory Vairables----
    public List<SavedSlot> savedSlotList = new List<SavedSlot>();
    
    //----Environment Variables----
    public List<string> openedChests = new List<string>();
    
    //----Quest Variables----
    public List<SavedQuest> savedQuests = new List<SavedQuest>();

}

/// <summary>
/// Holds the information of one Slot
/// </summary>
[System.Serializable]
public struct SavedSlot
{
    public int index; // Which Slot Index?
    public string itemID; 
    public int itemStackSize;
}

/// <summary>
/// Holds the information of one Quest
/// </summary>
[System.Serializable]
public struct SavedQuest
{
    public string questID;
    public int[] objectiveProgress;
    public bool isCompleted;
}