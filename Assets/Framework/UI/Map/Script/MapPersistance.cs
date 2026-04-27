using UnityEngine;

public class MapPersistance : MonoBehaviour, ISaveable
{
    [SerializeField] private MapManager mapManager;
    
    public void PopulateSaveData(SaveData saveData)
    {
    }

    public void LoadFromSaveData(SaveData saveData)
    {
    }
 
}
