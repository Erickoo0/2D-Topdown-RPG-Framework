using UnityEngine;

public class PlayerStatsPersistance : MonoBehaviour, ISaveable
{
    private GameObject _player;
    private Health _healthComponent;
    private Mana _manaComponent;
    private Level _levelComponent;
    
    private void Awake()
    {
        // Find the player game object
        _player = GameObject.FindGameObjectWithTag("Player");
        _healthComponent = GetComponent<Health>();
        _manaComponent = GetComponent<Mana>();
        _levelComponent = GetComponent<Level>();
    }

    public void PopulateSaveData(SaveData saveData)
    {
        saveData.playerPosition = transform.position;
        saveData.currentHealth = _healthComponent.HpCurrent;
        saveData.maxHealth = _healthComponent.hpMax;
        saveData.currentMana = _manaComponent.MpCurrent;
        saveData.maxMana = _manaComponent.mpMax;
        saveData.currentLevel = _levelComponent.LvlCurrent;
        saveData.currentExperience = _levelComponent.ExpCurrent;
    }
    
    public void LoadFromSaveData(SaveData saveData)
    {
        _player.transform.position = saveData.playerPosition;
        _healthComponent.HpCurrent = saveData.currentHealth;
        _manaComponent.MpCurrent = saveData.currentMana;
        _levelComponent.LvlCurrent = saveData.currentLevel;
        _levelComponent.ExpCurrent = saveData.currentExperience;
    }
}
