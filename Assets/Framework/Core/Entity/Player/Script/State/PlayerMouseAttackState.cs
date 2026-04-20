using UnityEngine;

[System.Serializable]
public class PlayerMouseAttackState : State<PlayerController>
{
    private MouseAttackData _attackData;
    private PlayerCombatController _combatController;
    private bool _isFinished;
    
    public override void Setup(PlayerController controller, StateMachine stateMachine)
    {
        base.Setup(controller, stateMachine);
        
        // Search the Combat Controller for the specific data asset
        _combatController = controller.GetComponent<PlayerCombatController>();
        
        if (_combatController != null)
        {
            _attackData = _combatController.GetAttackData<MouseAttackData>();
        }

        if (_attackData == null) 
            Debug.LogWarning($"{controller.gameObject.name} has MouseAttackState but no MouseAttackData in library.");

    }

    public void SetupContext(CombatContext combatContext)
    {
        //_context = combatContext;
    } 

    
    public override void Enter()
    {
        _isFinished = false;

        // Safety Check
        if (_attackData == null)
        {
            _isFinished = true;
            return;
        }
        
        // 1. Spawn Hitbox exactly at the mouse position
        HitBox spawnedHitbox = Object.Instantiate(_attackData.hitboxPrefab, _combatController.CombatContext.mousePosition, Quaternion.identity);
        spawnedHitbox.enableHitbox = true;
        
        // 2. Configure Hitbox Radius
        if (spawnedHitbox is HitBoxCircle circle) 
        { 
            circle.radius = _attackData.hitboxRadius; 
        }
        
        // 3. Trigger Visuals
        if (_attackData.attackFX != null)
        {
            GameObject fxInstance = Object.Instantiate(_attackData.attackFX, _combatController.CombatContext.mousePosition, Quaternion.identity);
            spawnedHitbox.ScaleVisual(fxInstance);
        }
        
        // 4. Process Damage using the injected context
        DamageData executionDamage = _attackData.damageData;
        executionDamage.source = _combatController.CombatContext.source;
        spawnedHitbox.CheckForHits(executionDamage);
        
        // 5. Clean up hierarchy immediately for an instant attack
        Object.Destroy(spawnedHitbox.gameObject);
        _isFinished = true;
    }
    
    public override void Update()
    {
        if (_isFinished) stateMachine.ChangeState(controller.IdleState);
    }
    
    public override void Exit() { }
    public override void PhysicsUpdate() { }
    public override void HandleInput() { }
}
