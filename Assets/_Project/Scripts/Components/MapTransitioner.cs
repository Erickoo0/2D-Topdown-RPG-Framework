using UnityEngine;
using Unity.Cinemachine;

public class MapTransitioner : MonoBehaviour
{
    [SerializeField] private BoxCollider2D targetMapBoundry;
    private CinemachineConfiner2D _confiner;

    [Header("Cooldown Settings")] 
    [SerializeField] private float transitionCooldown = 1.5f;
    
    // Static variable means ALL instances of this script share the same variable
    private static float _lastTransitionTIme;

    private void Awake()
    {
        _lastTransitionTIme = 0;
        // Assign the Camera Confiner
        _confiner = FindAnyObjectByType<CinemachineConfiner2D>();
        if (_confiner == null)
        {
            Debug.LogError("No confiner found");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if enough time has passed since the last transition by comparing current time to last transition time
        if (Time.time < _lastTransitionTIme + transitionCooldown)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player") && _confiner != null && targetMapBoundry != null)
        {
            _confiner.BoundingShape2D = targetMapBoundry;
            _confiner.InvalidateBoundingShapeCache();
            
            //U pdates shared timer to the current time
            _lastTransitionTIme = Time.time;
            
            // Tell MapManager to update its map tile
            MapManager.Instance?.HighlightTile(targetMapBoundry.name);
        }
    }
}
