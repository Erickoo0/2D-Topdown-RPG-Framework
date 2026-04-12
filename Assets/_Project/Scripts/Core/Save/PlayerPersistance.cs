using UnityEngine;

public class PlayerPersistance : MonoBehaviour, ISaveable
{
    private void OnEnable() => SaveManager.Instance.RegisterSaveable(this);
    private void OnDisable() => SaveManager.Instance.UnregisterSaveable(this);
    public void PopulateSaveData(SaveData data)
    {
        // We save the position from the transform this script is attached to
        data.playerPosition = transform.position;
        
        // You can add more player-specific save data here later
        // data.currentHealth = GetComponent<Health>().CurrentHealth;
    }

    public void LoadFromSaveData(SaveData data)
    {
        // Snap the player to the saved position
        transform.position = data.playerPosition;
    }
}
