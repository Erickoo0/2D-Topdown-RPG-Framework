using UnityEngine;

public class ScenePersistence : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() => DontDestroyOnLoad(gameObject);
}
