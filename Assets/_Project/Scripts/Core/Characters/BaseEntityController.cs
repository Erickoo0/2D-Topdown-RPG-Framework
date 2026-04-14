using UnityEngine;
using UnityEngine.InputSystem;

// A Data Hub that provides the API for states
[RequireComponent(typeof(EntityMover), typeof(EntityAnimator), typeof(StateMachine))]
public abstract class BaseEntityController : MonoBehaviour
{
    [Header("References")]
    public StateMachine StateMachine { get; private set; }
    public EntityMover EntityMover { get; private set; }
    public EntityAnimator EntityAnimator { get; private set; }
    

    protected virtual void Awake()
    {
        StateMachine = GetComponent<StateMachine>();
        EntityMover = GetComponent<EntityMover>();
        EntityAnimator = GetComponent<EntityAnimator>();
    } 
    
    protected virtual void Update()
    {
        if (PauseManager.IsGamePaused) return;
        
        StateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        if (PauseManager.IsGamePaused) return;

        StateMachine.FixedUpdate();
    }
}

