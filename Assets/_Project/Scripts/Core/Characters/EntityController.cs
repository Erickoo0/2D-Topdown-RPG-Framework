using UnityEngine;
using System.Collections.Generic;

public class EntityController : BaseEntityController
{
    [Header("AI Settings")] 
    public float aggroRange = 5f;
    public Transform target;
    
    [Header("Movement Data")]
    private Transform _waypointParent;
    public Vector2[] waypointsList;
    public int currentWaypointIndex = 0;
    public float defaultWaypointWaitTime = 2f;
    public bool loopWaypoints = true;
    
    [Header("State References")]
    public EntityIdleState  IdleState { get; private set; }
    public EntityWanderState WanderState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new EntityIdleState(this, StateMachine);
        WanderState = new EntityWanderState(this, StateMachine);
        
        SetupWaypointsList();
    }

    protected virtual void Start()
    {
        // Start by wandering to first waypoint
        StateMachine.SetupState(WanderState);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (target == null) target = other.transform;
        
        Vector2 lookDirection = (target.transform.position - transform.position).normalized;    
        EntityAnimator.FaceDirection(lookDirection);
        
        // If wandering, force idle state
        if (StateMachine.CurrentState == WanderState) StateMachine.ChangeState(IdleState);
    }

    //---- Helper Methods ----
    public bool IsTargetInRange()
    {
        if (target == null) return false;
        else return Vector2.Distance(transform.position, target.position) <= aggroRange;
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
}