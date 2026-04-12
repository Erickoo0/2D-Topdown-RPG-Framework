using UnityEngine;

public class MapPersistance : MonoBehaviour, ISaveable
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private CameraBoundaryController cameraBoundaryController;
    [SerializeField] private string defaultArea = "L1";
    
    private void OnEnable() => SaveManager.Instance.RegisterSaveable(this);
    private void OnDisable() => SaveManager.Instance.UnregisterSaveable(this);

    public void PopulateSaveData(SaveData saveData) => saveData.mapBoundaryName = mapManager.CurrentTileName;

    public void LoadFromSaveData(SaveData saveData)
    {
        string areaToLoad = string.IsNullOrEmpty(saveData.mapBoundaryName) ? defaultArea : saveData.mapBoundaryName;
        
        mapManager.HighlightTile(areaToLoad);
        cameraBoundaryController.UpdateCameraBoundary(areaToLoad);
    }
 
}
