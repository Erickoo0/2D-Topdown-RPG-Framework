using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles saving and loading player data and camera boundaries using JSON
/// </summary>
public class SaveManager : MonoBehaviour
{
    private string _savePath;

    private void Start()
    {
        // Define where the save file lives "persistentDataPath" is a special unity folder
        // that works across Windows, Mac, Andriod, and IOS
        _savePath = Path.Combine(Application.persistentDataPath, "Save.json");
        
        // Load the game at start
        LoadGame();
    }

    public void SaveGame()
    {
        // Create a new instance of our data container and fill it with current data
        SaveData saveData = new SaveData();
        
        // Find everything that wants to be saved
        var saveables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();

        foreach (var s in saveables)
        {
            s.PopulateSaveData(saveData);
        }
        
        // Convert the SaveData object into a JSON string (text)
        string json = JsonUtility.ToJson(saveData);
        
        // Write that text to the hard drive file location _savePath
        File.WriteAllText(_savePath, json);
        
        Debug.Log("Game Saved to: {_savePath}");
    }

    public void LoadGame()
    {
        // 1. Check if the file actually exists
        if (!File.Exists(_savePath))
        {
            Debug.Log("No save file found. Creating a fresh one.");
            SaveGame(); // Create an initial save if none exists
            return;
        }

        // 2. Read the text and turn it back into our data object
        string json = File.ReadAllText(_savePath);
        SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

        // 3. Find every script in the scene that implements ISaveable
        var allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
    
        foreach (var script in allScripts)
        {
            if (script is ISaveable saveable)
            {
                // 4. Pass the loaded box to the script so it can restore itself
                saveable.LoadFromSaveData(loadedData);
            }
        }

        Debug.Log("Game Loaded Successfully via ISaveable!");
    }
}

