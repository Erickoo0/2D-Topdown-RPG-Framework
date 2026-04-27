using UnityEngine;

[DisallowMultipleComponent]
public class UniqueIdentifier : MonoBehaviour
{
    [SerializeField] private string id;
    public string ID => id;

    private void Awake()
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError($"Missing UniqueIdentifier ID on {name}. This object will not save/load consistently.", this);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            return;
        }

        // Check for duplicate ids
        UniqueIdentifier[] otherIds = FindObjectsByType<UniqueIdentifier>(FindObjectsSortMode.None);
        foreach (UniqueIdentifier otherId in otherIds)
        {
            if (otherId != this && otherId.ID == id)
            {
                id = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
                break;
            }
        }
    }

    [ContextMenu("Force Regenerate ID")]
    private void RegenerateId()
    {
        id = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}