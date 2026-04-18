using UnityEngine;
using System.Collections.Generic;

public enum MobType { Passive, Neutral, Aggressive }

public class EntityController : BaseEntityController
{
    [Header("Mob Type & Targeting")] 
    [field: SerializeField] public MobType mobType { get; private set; }  = MobType.Aggressive;
    [field: SerializeField] public float DetectionRange { get; private set; } = 6f;
    [field: SerializeField] public float DetectionLostRange { get; private set; } = 8f;
    [field: SerializeField] public float ActionRange { get; private set; } = 5f;
    [field: SerializeField] public List<string> TargetableList { get; private set; }
    public Transform currentTarget ;
    
    [Header("Action Settings")] 
    [field: SerializeField] public EntityActionModule EntityAction { get; private set; }
    [field: SerializeField] public float ActionCooldown  { get; private set; } = 2f;
    private float _lastActionTime;

    
    [Header("Movement Data")]
    [field: SerializeField] public float DefaultWaypointWaitTime { get; private set; } = 2f;
    [field: SerializeField] public bool LoopWaypoints { get; private set; } = true;
    private Transform _waypointParent;
    public Vector2[] waypointsList;
    public int currentWaypointIndex = 0;
    
    [Header("State References")]
    public EntityIdleState  IdleState { get; private set; }
    public EntityWanderState WanderState { get; private set; }
    public EntityChaseState ChaseState { get; private set; }
    public EntityActionState ActionState { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        
        IdleState = new EntityIdleState(this, StateMachine);
        WanderState = new EntityWanderState(this, StateMachine);
        ChaseState = new EntityChaseState(this, StateMachine);
        ActionState = new EntityActionState(this, StateMachine, EntityAction);
        
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

        UpdateTargeting();
    }

    //---- Targeting Methods ----
    private void UpdateTargeting()
    {
        if (currentTarget == null)
        {
            FindTarget();
            return;
        }

        if (!IsTargetInRange(DetectionLostRange))
        {
            ClearTarget();
        }
    }

    private void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, DetectionRange);

        // Check all collided instances if they are targetable
        foreach (Collider2D hit in hits)
        {
            ITargetable targetInterface = hit.GetComponentInParent<ITargetable>();

            if (targetInterface == null) continue;
            if (TargetableList == null || !TargetableList.Contains((targetInterface.GetTargetID()))) continue;
            
            // Set the target
            currentTarget = hit.transform;
            StateMachine.ChangeState(ChaseState);
            return;
        }
    }
    
    private void ClearTarget()
    {
        currentTarget = null;
        StateMachine.ChangeState(IdleState);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (StateMachine.CurrentState != ChaseState && StateMachine.CurrentState != ActionState)
        {
            Vector2 lookDirection = (other.transform.position - transform.position).normalized;    
            EntityAnimator.FaceDirection(lookDirection);
            
            // If wandering, force idle state
            if (StateMachine.CurrentState == WanderState) StateMachine.ChangeState(IdleState);
        }
    }
    
    private bool IsTargetInRange(float range)
    {
        if (currentTarget == null) return false;
        else return Vector2.Distance(transform.position, currentTarget.transform.position) <= range;
    }
    
    
    //---- Waypoint Methods ----
    private void SetupWaypointsList()
    {
        // Find the childed "Waypoint Parent" object
        if (_waypointParent == null) _waypointParent = transform.Find("Waypoint Parent");
        
        // If there is no childed Waypoint Parent, return
        if (_waypointParent == null) return;
        
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
        if (LoopWaypoints) currentWaypointIndex = (currentWaypointIndex + 1) % waypointsList.Length;
        else if (currentWaypointIndex < waypointsList.Length - 1) currentWaypointIndex++;
    } 
    
    
    //---- Action Methods -----
    public bool CheckActionCooldown() 
    {
        return Time.time >= _lastActionTime + ActionCooldown;
    }
    
    // A method to reset the timer (called when the action finishes)
    public void SetActionCooldown()
    {
        _lastActionTime = Time.time;
    }
}