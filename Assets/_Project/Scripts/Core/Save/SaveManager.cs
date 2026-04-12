using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles saving and loading player data and camera boundaries using JSON
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    private string _savePath;
    private readonly List<ISaveable> _saveables = new List<ISaveable>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Define where the save file lives "persistentDataPath" is a special unity folder
        _savePath = Path.Combine(Application.persistentDataPath, "Save.json");

    }
    
    // Called from ISaveable Interfaces
    public void RegisterSaveable(ISaveable saveable) => _saveables.Add(saveable);
    public void UnregisterSaveable(ISaveable saveable) => _saveables.Remove(saveable);

    //---- Scene Transition Logic----
    public void TransitionScene(string sceneName, bool isNewGame)
    {
        StartCoroutine(TransitionRoutine(sceneName, isNewGame));
    }

    private IEnumerator TransitionRoutine(string sceneName, bool isNewGame)
    {
        // 1. Begin Loading Scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // 2. Wait here untill Unity completely finishes loading the scene
        if (asyncLoad != null)
            while (!asyncLoad.isDone) yield return null;
        
        // 3. Proceed with Load & Save Logic
        if (isNewGame) NewGame();
        else LoadGame();
    }
    
    //---- File Load & Save Logic----
    public void NewGame()
    {
        Debug.Log("Starting New Game");
        // 1. Delete previous save data
        if (File.Exists(_savePath)) File.Delete(_savePath);
        // 2. Create a new save data
        SaveData newSaveData = new SaveData();
        // 3. Import new save data into all ISaveables
        foreach (ISaveable saveable in _saveables) saveable.LoadFromSaveData(newSaveData);
        // 4. Save the new game file
        SaveGame();
 
    }
    
    public void SaveGame()
    {
        // 1. Create a container to hold save data
        SaveData saveData = new SaveData();
        // 2. Export all save data from ISaveable into container
        foreach (ISaveable saveable in _saveables) saveable.PopulateSaveData(saveData);
        // 3. Convert the SaveData object into a JSON string (text)
        string json = JsonUtility.ToJson(saveData);
        // 4. Write that text to the hard drive file location _savePath
        File.WriteAllText(_savePath, json);
        Debug.Log($"Game Saved to: {_savePath}");
    }

    public void LoadGame()
    {
        // 1. Check if the file actually exists
        if (!File.Exists(_savePath))
        {
            Debug.Log("No save file found.");
            return;
        }
        // 2. Read the text from our save path and turn it back into our data object
        string json = File.ReadAllText(_savePath);
        SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
        // 3. Find every script in the scene that implements ISaveable
        var allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        // 4. Pass the save data into each ISaveable script
        foreach (ISaveable saveable in _saveables) saveable.LoadFromSaveData(loadedData);
        Debug.Log("Game Loaded Successfully!");
    }
}

