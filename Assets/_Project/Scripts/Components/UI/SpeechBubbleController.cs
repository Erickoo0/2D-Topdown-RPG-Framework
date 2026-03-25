using TMPro;
using UnityEngine;
using System.Collections;

public class SpeechBubbleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private string[] speechBubbleLines;
    
    [Header("Speech Bubble Settings")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private float speechBubbleTypeSpeed = 0.05f;
    [SerializeField] private float speechBubbleMinWaitTime = 3f;
    [SerializeField] private float speechBubbleMaxWaitTime = 9f;
    [SerializeField] private float speechBubbleDuration = 2.5f;

    private float timer;
    private GameObject currentBubble;
    private TypeWriter typeWriter;
    
    private void Start() => SetRandomWaitTime();

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (currentBubble != null) DespawnBubble();
            else SpawnBubble();
        }
    }

    private void SpawnBubble()
    {
        // Safety Check
        if (speechBubbleLines.Length == 0 || speechBubblePrefab == null) return;
        
        // 1. Create bubble and parent it to NPC with offset
        currentBubble = Instantiate(speechBubblePrefab, transform.position + spawnOffset, Quaternion.identity, transform);
        
        // 2. Find the TextMeshPro component in the children and set the text
        TypeWriter typewriter = currentBubble.GetComponentInChildren<TypeWriter>();
        if (typewriter != null) typewriter.StartTyping(speechBubbleLines[Random.Range(0, speechBubbleLines.Length)]);
        
        // 3. Set alive timer 
        timer = speechBubbleDuration;
    }

    private void DespawnBubble()
    {
        Destroy(currentBubble);
        currentBubble = null;
        SetRandomWaitTime();
    }
    
    private void SetRandomWaitTime() => timer = Random.Range(speechBubbleMinWaitTime, speechBubbleMaxWaitTime);
    
}
