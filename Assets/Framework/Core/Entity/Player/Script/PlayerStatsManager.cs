using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    [Header("Player Stats")] [SerializeField]
    private Health healthComponent;
    private Mana manaComponent;
    private Level levelComponent;

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
        manaComponent = GetComponent<Mana>();
        levelComponent = GetComponent<Level>();
        
        UpdateStatsMenu();
    }

    private void OnEnable()
    {
        // Catch and discard the broadcasted values as we dont need them here
        healthComponent.OnHpUpdated += (hp) => UpdateStatsMenu();
        manaComponent.OnMpUpdated += (mp) => UpdateStatsMenu();
        levelComponent.OnLevelUpdated += (level) => UpdateStatsMenu();
        levelComponent.OnExperienceGained += (curr, total) => UpdateStatsMenu();

        EventBus.OnEntityDeathRequested += HandleEntityDeath;
    }
    
    private void OnDisable()
    {
        healthComponent.OnHpUpdated -= (hp) => UpdateStatsMenu();
        manaComponent.OnMpUpdated -= (mp) => UpdateStatsMenu();
        levelComponent.OnLevelUpdated -= (level) => UpdateStatsMenu();
        levelComponent.OnExperienceGained -= (curr, total) => UpdateStatsMenu();
        EventBus.OnEntityDeathRequested -= HandleEntityDeath;
    }

    private void HandleEntityDeath(GameObject entity)
    {
        if (entity.TryGetComponent(out Level entityLevelComponent))
        {
            levelComponent.AddExperience(entityLevelComponent.ExpYield);
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
        playerMpText.text = ($"MP: {manaComponent.MpCurrent}/{manaComponent.mpMax}");
        playerLvlText.text = ($"Lvl: {levelComponent.LvlCurrent}");
        playerExpText.text = ($"Exp: {levelComponent.ExpCurrent}/{levelComponent.ExpToNextLvl}");
    }

}
