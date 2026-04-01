using System.Collections.Generic;
using UnityEngine;

public class EnableUI : MonoBehaviour
{
    [Header("Exclusion Settings")]
    [Tooltip("Drag children here that you want to remain HIDDEN/DISABLED on Start.")]
    [SerializeField] private List<GameObject> excludedChildren = new List<GameObject>();

    private void Start()
    {
        // Loop through all children of this transform
        foreach (Transform child in transform)
        {
            // Skip children on the exclusion list
            if (excludedChildren.Contains(child.gameObject)) continue;
            // Set all other children to active
            child.gameObject.SetActive(true);
        }
    }
}