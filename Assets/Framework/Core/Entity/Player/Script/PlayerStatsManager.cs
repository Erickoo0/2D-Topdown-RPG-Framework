using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    [Header("Player Stats")] [SerializeField]
    private Health healthComponent;
    private Mana _manaComponent;
    private Level _levelComponent;

    [Header("UI References")] [SerializeField]
    private GameObject playerStatsPanel;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI playerMpText;
    [SerializeField] private TextMeshProUGUI playerArmorText;
    [SerializeField] private TextMeshProUGUI playerLvlText;
    [SerializeField] private TextMeshProUGUI playerExpText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.unityLogger.Log("Multiple PlayerStatsManagers detected. Disabling script.");
            return;
        }

        Instance = this;

        // Get the components
        healthComponent = GetComponent<Health>();
        _manaComponent = GetComponent<Mana>();
        _levelComponent = GetComponent<Level>();
        
        UpdateStatsMenu();
    }

    private void OnEnable()
    {
        // Catch and discard the broadcasted values as we dont need them here
        healthComponent.OnHpUpdated += UpdateStatsMenu;
        _manaComponent.OnMpUpdated += UpdateStatsMenu;
        _levelComponent.OnLevelUpdated += UpdateStatsMenu;
        _levelComponent.OnExperienceGained += UpdateStatsMenu;

        EventBus.OnEntityDeathRequested += HandleEntityDeath;
    }
    
    private void OnDisable()
    {
        healthComponent.OnHpUpdated -= UpdateStatsMenu;
        _manaComponent.OnMpUpdated -=  UpdateStatsMenu;
        _levelComponent.OnLevelUpdated -= UpdateStatsMenu;
        _levelComponent.OnExperienceGained -= UpdateStatsMenu;
        EventBus.OnEntityDeathRequested -= HandleEntityDeath;
    }

    private void HandleEntityDeath(GameObject entity)
    {
        if (entity.TryGetComponent(out Level entityLevelComponent))
        {
            _levelComponent.AddExperience(entityLevelComponent.ExpYield);
        }
    }
    
    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player is not null)
            transform.position = player.transform.position;
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!playerStatsPanel.activeSelf) EventBus.RequestOpenMenu(playerStatsPanel);
        else if (playerStatsPanel.activeSelf) EventBus.RequestCloseMenu(playerStatsPanel);
    }

    private void UpdateStatsMenu()
    {
        playerHpText.text = ($"HP: {healthComponent.HpCurrent}/{healthComponent.hpMax}");
        playerMpText.text = ($"MP: {_manaComponent.MpCurrent}/{_manaComponent.mpMax}");
        playerLvlText.text = ($"Lvl: {_levelComponent.LvlCurrent}");
        playerExpText.text = ($"Exp: {_levelComponent.ExpCurrent}/{_levelComponent.ExpToNextLvl}");
    }

}
