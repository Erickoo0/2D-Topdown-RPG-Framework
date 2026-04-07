using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaypointMover : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float maxStartDelay = 4f;
    [SerializeField] private bool loopWaypoints = true;

    [Header("Waypoint Settings")]
    private Transform _waypointParent;
    private Vector2[] _waypoints;
    private int _currentWaypointIndex;
    [Header("Animation Settings")]
    private EntityAnimationController _entityAnimation;
    private bool _isWaiting;
    

    private void Start()
    {
        // 1. Get the Animator 
        _entityAnimation = GetComponent<EntityAnimationController>();
        
        // 2. Get the waypoint parent
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
            _entityAnimation?.StopAnimation();            
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
            // 1. Calculate direction
            Vector2 moveDirection = (target - currentPosition).normalized;
            
            // 2. Tell the animation controller to update
            _entityAnimation?.UpdateAnimation(moveDirection);
            
            // 3. Move
            transform.position = Vector2.MoveTowards(currentPosition, target, moveSpeed * Time.deltaTime);
        }
        else
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        // Idle for duration
        _isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        
        // Check if target was reached (if stopped because of collision)
        float distance = Vector2.Distance(transform.position, _waypoints[_currentWaypointIndex]);
        
        // If we reached the target, move target to next waypoint
        if (distance <= 0.1f) _currentWaypointIndex = loopWaypoints ? (_currentWaypointIndex + 1) % _waypoints.Length : _currentWaypointIndex;
        
        _isWaiting = false;

    }

    IEnumerator RandomStartDelay()
    {
        _isWaiting = true;
        float randomDelay = Random.Range(0.2f, maxStartDelay);
        yield return new WaitForSeconds(randomDelay);
        _isWaiting = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isWaiting)
        {
            StartCoroutine(WaitAtWaypoint());
        }
        Vector2 lookDirection = (collision.transform.position - transform.position).normalized;
        _entityAnimation?.FaceDirection(lookDirection);
    }
}
