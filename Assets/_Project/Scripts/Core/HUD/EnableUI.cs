using UnityEngine;

public class EnableUI : MonoBehaviour
{
    private void Start()
    {
        // Iterate through all immediate children of this transform
        foreach (Transform child in transform)
        {
            // Set the child GameObject to active
            child.gameObject.SetActive(true);
        }
    }
}