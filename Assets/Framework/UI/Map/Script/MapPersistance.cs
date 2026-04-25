using UnityEngine;

public class MapPersistance : MonoBehaviour, ISaveable
{
    [SerializeField] private MapManager mapManager;
    
    private void OnEnable() => SaveManager.Instance.RegisterSaveable(this);
    private void OnDisable() => SaveManager.Instance.UnregisterSaveable(this);

    public void PopulateSaveData(SaveData saveData)
    {
    }

    public void LoadFromSaveData(SaveData saveData)
    {
    }
 
}
