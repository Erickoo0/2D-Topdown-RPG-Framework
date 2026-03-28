using UnityEngine;
using System.Collections.Generic;


public class FloatingManager : MonoBehaviour
{
    [SerializeField] private FloatingText textPrefab; // The object to spawn
    [SerializeField] private int poolSize; // Bigger Pool = More Memory
    [SerializeField] private float yOffset = 1.5f;
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float randomYRange = 0.2f;
    [SerializeField] private float randomXRange = 0.5f;


    // Custom Object Pool using a Queue
    private readonly Queue<FloatingText> textPool = new Queue<FloatingText>();


    private void Awake()
    {
        // Pre-Create Object Pool
        for (int i = 0; i < poolSize; i++)
        {
            FloatingText newText = CreateTextObject();
            textPool.Enqueue(newText); // Put them in the Queue
        }
    }
    
    // Subscribes to CombatEvents
    private void OnEnable() => EventBus.OnFloatingTextRequested += SpawnFloatingNumber;
    private void OnDisable() => EventBus.OnFloatingTextRequested -= SpawnFloatingNumber;


    private void SpawnFloatingNumber(int amount, Vector3 position)
    {
        FloatingText textObject;


        // Check if Queue has available objects
        if (textPool.Count > 0)
        {
            textObject = textPool.Dequeue(); // Pull from the front of the line
        }
        else // If the Queue is empty (too many numbers on screen), make a new object
        {
            textObject = CreateTextObject();
        }
        
        // Apply Y Offset to position
        position.y += yOffset + Random.Range(-randomYRange, randomYRange);
        position.x += xOffset + Random.Range(-randomXRange, randomXRange);
        
        // Tell the text to turn on
        // Pass the amount, position, and reference to the "ReturnToPool" method
        textObject.Initialize(amount, position, ReturnToPool);
    }


    // Called when the Text Object finishes animating
    private void ReturnToPool(FloatingText textObject)
    {
        textObject.gameObject.SetActive(false);
        textPool.Enqueue(textObject); // Put an object at the end of the line
    }


    // Helper Method
    private FloatingText CreateTextObject()
    {
        FloatingText newText = Instantiate(textPrefab, transform);
        newText.gameObject.SetActive(false);
        return newText;
    }
}

