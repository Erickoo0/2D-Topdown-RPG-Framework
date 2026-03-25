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
    private Transform waypointParent;
    private Vector2[] waypoints;
    private int currentWaypointIndex;
    [Header("Animation Settings")]
    private EntityAnimationController _entityAnimation;
    private CircleCollider2D _entityCollider;
    private bool isWaiting;
    

    private void Start()
    {
        // 1. Get the Collider
        _entityCollider = GetComponent<CircleCollider2D>();
        
        // 2. Get the Animator 
        _entityAnimation = GetComponent<EntityAnimationController>();
        
        // 3. Get the waypoint parent
        if (waypointParent == null) waypointParent = transform.Find("Waypoint Parent");

        if (waypointParent != null)
        {
            SetupWaypoints();
            StartCoroutine(RandomStartDelay());
        }

    }
    
    private void SetupWaypoints()
    {
        waypoints = new Vector2[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++) 
        {
            waypoints[i] = waypointParent.GetChild(i).position;
        }
    }
    
    private void Update()
    {
        if (PauseManager.IsGamePaused || isWaiting || waypoints == null)
        {
            _entityAnimation?.StopAnimation();            
            return;
        }
        MoveToWaypoint();
    }
    
    private void MoveToWaypoint()
    {
        Vector2 target = waypoints[currentWaypointIndex]; // Get the current waypoint position
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
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        
        // Check if target was reached (if stopped because of collision)
        float distance = Vector2.Distance(transform.position, waypoints[currentWaypointIndex]);
        
        // If we reached the target, move target to next waypoint
        if (distance <= 0.1f) currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : currentWaypointIndex;
        
        isWaiting = false;

    }

    IEnumerator RandomStartDelay()
    {
        isWaiting = true;
        float randomDelay = Random.Range(0.2f, maxStartDelay);
        yield return new WaitForSeconds(randomDelay);
        isWaiting = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isWaiting)
        {
            StartCoroutine(WaitAtWaypoint());
        }
        Vector2 lookDirection = (collision.transform.position - transform.position).normalized;
        _entityAnimation?.FaceDirection(lookDirection);
    }
}
