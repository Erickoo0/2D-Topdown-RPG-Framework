using Unity.Cinemachine;
using UnityEngine;

public class CameraPersistance : MonoBehaviour, ISaveable
{
    private CinemachineConfiner2D _confiner;

    // This property ensures _confiner is never null (race condition against SaveController Awake() if we used Awake())
    private CinemachineConfiner2D Confiner
    {
        get
        {
            if (_confiner == null) _confiner = GetComponent<CinemachineConfiner2D>();
            return _confiner;
        }
    }
    

    public void PopulateSaveData(SaveData saveData)
    {
        // If the _confiner has a BoundingShape2D save its Name to JSON, otherwise pass an empty string
        saveData.mapBoundaryName = Confiner.BoundingShape2D != null ? Confiner.BoundingShape2D.gameObject.name : "";
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        // Check if a mapBoundaryName exists
        if (!string.IsNullOrEmpty(saveData.mapBoundaryName))
        {
            // Search the entire scene for a game object matching the name
            GameObject boundaryObject = GameObject.Find(saveData.mapBoundaryName);
            if (boundaryObject != null)
            {
                // Assign current camera BoundingShape2D to the one in SaveData
                Confiner.BoundingShape2D = boundaryObject.GetComponent<Collider2D>();
                Confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}
