using Unity.Cinemachine;
using UnityEngine;

public class CameraPersistance : MonoBehaviour, ISaveable
{
    private CinemachineConfiner2D _confiner;
     
    private void Awake() => _confiner = GetComponent<CinemachineConfiner2D>();

    public void PopulateSaveData(SaveData saveData)
    {
        // If the _confiner has a BoundingShape2D save its Name to JSON, otherwise pass an empty string
        saveData.mapBoundaryName = _confiner.BoundingShape2D != null ? _confiner.BoundingShape2D.gameObject.name : "";
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
                _confiner.BoundingShape2D = boundaryObject.GetComponent<Collider2D>();
                _confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}
