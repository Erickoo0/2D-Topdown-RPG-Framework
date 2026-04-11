using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaypointMover : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float maxStartDelay = 4f;
    [SerializeField] private bool loopWaypoints = true;

    [Header("Waypoint Settings")]
    private Transform _waypointParent;
    private Vector2[] _waypoints;
    private int _currentWaypointIndex;
    
    private EntityMover _entityMover;
    private EntityAnimator _entityAnimator;
    private bool _isWaiting;


    private void Awake()
    {
        _entityMover = GetComponent<EntityMover>();
        _entityAnimator = GetComponent<EntityAnimator>();
    }
    
    private void Start()
    {
        // Setup waypoint parent if not assigned in inspector
        if (_waypointParent == null) _waypointParent = transform.Find("Waypoint Parent");

        if (_waypointParent != null)
        {
            SetupWaypoints();
            StartCoroutine(RandomStartDelay());
        }
    }
    
    private void SetupWaypoints()
    {
        _waypoints = new Vector2[_waypointParent.childCount];
        for (int i = 0; i < _waypointParent.childCount; i++) 
        {
            _waypoints[i] = _waypointParent.GetChild(i).position;
        }
    }
    
    private void Update()
    {
        if (PauseManager.IsGamePaused || _isWaiting || _waypoints == null)
        {
            _entityMover.SetMoveDirection(Vector2.zero);          
            return;
        }
        
        MoveToWaypoint();
    }
    
    private void MoveToWaypoint()
    {
        Vector2 target = _waypoints[_currentWaypointIndex]; // Get the current waypoint position
        Vector2 currentPosition = transform.position;
        float distance = Vector2.Distance(currentPosition, target);
        
        // Move to waypoint 
        if (distance > 0.1f)
        {
            // Calculate direction and pass it to the Entity Mover
            Vector2 moveDirection = (target - currentPosition).normalized;
            _entityMover.SetMoveDirection(moveDirection);
        }
        else
        {   
            // Stop the movement and wait once we reach the destination
            _entityMover.SetMoveDirection(Vector2.zero);
            StartCoroutine(WaitAtWaypoint(true));
        }
    }

    private IEnumerator WaitAtWaypoint(bool reachedDestination)
    {
        // Idle for duration
        _isWaiting = true;
        _entityMover.SetMoveDirection(Vector2.zero);
        
        yield return new WaitForSeconds(waitTime);
        
        // Advance the waypoint index
        if (reachedDestination) _currentWaypointIndex = loopWaypoints ? (_currentWaypointIndex + 1) % _waypoints.Length : _currentWaypointIndex;
        
        _isWaiting = false;
    }

    private IEnumerator RandomStartDelay()
    {
        _isWaiting = true;
        float randomDelay = Random.Range(0.2f, maxStartDelay);
        yield return new WaitForSeconds(randomDelay);
        _isWaiting = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        if (!_isWaiting)
        {
            // Stop movement
            _entityMover.SetMoveDirection(Vector2.zero);
            StartCoroutine(WaitAtWaypoint(false));
        }
        // Look at player / object
        Vector2 lookDirection = (collision.transform.position - transform.position).normalized;
        _entityAnimator?.FaceDirection(lookDirection);
    }
}
