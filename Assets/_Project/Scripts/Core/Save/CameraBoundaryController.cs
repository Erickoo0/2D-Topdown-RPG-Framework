using Unity.Cinemachine;
using UnityEngine;

public class CameraBoundaryController : MonoBehaviour
{
    private CinemachineConfiner2D _confiner;
    
    private void Awake() =>  _confiner = GetComponent<CinemachineConfiner2D>();

    public void UpdateCameraBoundary(string areaName)
    {
        // Search the entire scene for a game object matching the name
        GameObject boundaryObject = GameObject.Find(areaName);

        // If found, set camera boundaries to match it
        if (boundaryObject != null)
        {
            _confiner.BoundingShape2D = boundaryObject.GetComponent<Collider2D>();
            _confiner.InvalidateBoundingShapeCache();
        }
    }
}
