using UnityEngine;
using UnityEngine.InputSystem;

// A Data Hub that provides the API for states
[RequireComponent(typeof(EntityMover), typeof(EntityAnimator), typeof(StateMachine))]
public abstract class BaseEntityController : MonoBehaviour, ITargetable
{
    [Header("ID")] 
    [SerializeField] private string entityID;
    public string GetTargetID() => entityID;
    
    [Header("References")] 
    protected StateMachine StateMachine { get; private set; }
    public EntityMover EntityMover { get; private set; }
    public EntityAnimator EntityAnimator { get; private set; }
    

    protected virtual void Awake()
    {
        // Get all the components
        StateMachine = GetComponent<StateMachine>();
        EntityMover = GetComponent<EntityMover>();
        EntityAnimator = GetComponent<EntityAnimator>();
    } 
    
    protected virtual void Update()
    {
        if (PauseManager.IsGamePaused) return;
        
        StateMachine.UpdateState(); // Link the State Machine Update method to Unity Update
    }

    protected virtual void FixedUpdate()
    {
        if (PauseManager.IsGamePaused) return;

        StateMachine.FixedUpdateState(); // Link the State Machine Fixed Update method to Unity FixedUpdate
    }
}

