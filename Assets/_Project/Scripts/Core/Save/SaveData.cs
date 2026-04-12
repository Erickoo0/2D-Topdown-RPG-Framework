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
    public int currentHealth;
    public int maxHealth;
    public int currentMana;
    public int maxMana;
    
    //----Area Variables----
    public string mapBoundaryName; //Stores Boundary Name
    
    //----Inventory Vairables----
    public List<SavedSlot> savedSlotList = new List<SavedSlot>();
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