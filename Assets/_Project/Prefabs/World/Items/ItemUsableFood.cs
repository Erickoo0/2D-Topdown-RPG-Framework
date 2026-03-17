using UnityEngine;

public class ItemUsableFood : MonoBehaviour, IUsable
{
    [SerializeField] private int healthHealAmount = 10;
    [SerializeField] private int manaHealAmount = 10;
    private GameObject _player;
    private Health _health;
    private Mana _mana;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _health = _player.GetComponent<Health>();
        _mana = _player.GetComponent<Mana>();
    }
    public void Use()
    {
        if (healthHealAmount > 0)
        {
            _health.HealHeath(healthHealAmount);
            Debug.unityLogger.Log($"{this.name} has been consumed, {_player.name} health healed by {healthHealAmount}.");
        }

        if (manaHealAmount > 0)
        {
            _mana.HealMana(manaHealAmount);
            Debug.unityLogger.Log($"{this.name} has been consumed, {_player.name} mana healed by {manaHealAmount}.");

        }
        Destroy(gameObject);
    }
}
