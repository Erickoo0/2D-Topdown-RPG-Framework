using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Vector2[] waypoints;
    private int currentWaypointIndex;
    private bool isWaiting;

    private void Start()
    {
        waypoints = new Vector2[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i).position;
        }
    }

    private void Update()
    {
        if (PauseManager.IsGamePaused || isWaiting) return;
        
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Vector2 target = waypoints[currentWaypointIndex];
        
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            // Wait at waypoint
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        
        // If looping is enabled, increment the waypoint index and wrap around if needed
        // Otherwise, only increment till last waypoint
        currentWaypointIndex = loopWaypoints? (currentWaypointIndex + 1) % waypoints.Length : currentWaypointIndex;
        isWaiting = false;
    }
}
