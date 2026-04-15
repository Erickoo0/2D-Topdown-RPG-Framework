using UnityEngine;
using System.Collections.Generic;

public enum MobType { Passive, Neutral, Aggressive }

public class EntityController : BaseEntityController
{
    [Header("Mob Type & Targeting")] 
    public MobType mobType = MobType.Aggressive;
    public float detectionRange = 6f;
    public float detectionLostRange = 8f;
    public float actionRange = 5f;
    public Transform currentTarget;
    public List<string> targetableList;

    [Header("Action Settings")] 
    public float actionCooldown = 2f;
    private float _lastActionTime;
    
    [Header("Movement Data")]
    private Transform _waypointParent;
    public Vector2[] waypointsList;
    public int currentWaypointIndex = 0;
    public float defaultWaypointWaitTime = 2f;
    public bool loopWaypoints = true;
    
    [Header("Action Module")]
    [SerializeField] private EntityActionModule entityAction;
    
    [Header("State References")]
    public EntityIdleState  IdleState { get; private set; }
    public EntityWanderState WanderState { get; private set; }
    public EntityChaseState ChaseState { get; private set; }
    public EntityActionState ActionState { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();

        // Grab the action component if we forgot to assign it
        //if (entityAction == null) entityAction = GetComponent<EntityAction>();
        
        IdleState = new EntityIdleState(this, StateMachine);
        WanderState = new EntityWanderState(this, StateMachine);
        ChaseState = new EntityChaseState(this, StateMachine);
        ActionState = new EntityActionState(this, StateMachine, entityAction);
        
        SetupWaypointsList();
    }

    protected virtual void Start()
    {
        // Start by wandering to first waypoint
        StateMachine.SetupState(WanderState);
    }

    protected override void Update()
    {
        base.Update();

        if (currentTarget == null) FindTarget();
        else if (currentTarget != null) CheckLoseTarget();
    }

    private void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        // Check all collided instances if they are targetable
        foreach (Collider2D hit in hits)
        {
            ITargetable targetInterface = hit.GetComponentInParent<ITargetable>();

            // Check if the targetable instance matches targetableList
            if (targetInterface != null && targetableList.Contains(targetInterface.GetTargetID()))
            {
                // Assign the target
                currentTarget = hit.transform; 
                StateMachine.ChangeState(ChaseState);
                return;
            }
        }
    }
    
    private void CheckLoseTarget()
    {
        if (Vector2.Distance(transform.position, currentTarget.position) > detectionLostRange)
        {
            currentTarget = null;
            StateMachine.ChangeState(WanderState);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        Vector2 lookDirection = (other.transform.position - transform.position).normalized;    
        EntityAnimator.FaceDirection(lookDirection);
        
        // If wandering, force idle state
        if (StateMachine.CurrentState == WanderState) StateMachine.ChangeState(IdleState);
    }

    //---- Helper Methods ----
    public bool IsTargetInRange()
    {
        if (currentTarget == null) return false;
        else return Vector2.Distance(transform.position, currentTarget.transform.position) <= detectionRange;
    }

    private void SetupWaypointsList()
    {
        // Find the childed "Waypoint Parent" object
        if (_waypointParent == null) _waypointParent = transform.Find("Waypoint Parent");
        
        // Get the childed objects of "Waypoint Parent" and save their positions in a list
        waypointsList = new Vector2[_waypointParent.childCount];
        for (int i = 0; i < _waypointParent.childCount; i++) 
        {
            waypointsList[i] = _waypointParent.GetChild(i).position;
        }

    }
    
    public Vector2 GetCurrentWaypointPosition()
    {
        // If no waypoints, stand still
        if (waypointsList == null || waypointsList.Length <= 0) return transform.position;
        // Return waypoint position
        return waypointsList[currentWaypointIndex];
    }

    public void AdvanceToNextWaypoint()
    {
        // Safety check
        if (waypointsList == null || waypointsList.Length <= 0) return;
        
        // Advance the waypoint
        if (loopWaypoints) currentWaypointIndex = (currentWaypointIndex + 1) % waypointsList.Length;
        else if (currentWaypointIndex < waypointsList.Length - 1) currentWaypointIndex++;
    }
    
    // A helper method to check if we can act
    public bool CanPerformAction() 
    {
        return Time.time >= _lastActionTime + actionCooldown;
    }
    
    // A method to reset the timer (called when the action finishes)
    public void ResetActionCooldown()
    {
        _lastActionTime = Time.time;
    }
}