using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh; // Reference to the Text game object
    [SerializeField] private float lifetime = 3f;
    private Action<FloatingText> _returnToPool; // Stores the "ReturnToPool" method from the FloatingManager
    private string text; // The text to display
    
    // The Manager calls this when grabbing the text from the queue
    public void Initialize(int amount, Vector3 spawnPosition, Action<FloatingText> returnAction)
    {
        _returnToPool = returnAction; // Save the return method from FloatingManager for use later
        
        //Check if amount is positive or negative
        if (amount >= 0)
        {
            text = "+" + amount;
            textMesh.color = Color.green;
        }
        else
        {
            text = amount.ToString();
            textMesh.color = Color.red;
        }
        
        // Reset the State
        transform.position = spawnPosition;
        textMesh.text = text;
        textMesh.alpha = 1f;
        gameObject.SetActive(true);
        
        Animate();
    }
    
    private void Animate()
    {
        // Float up by 1.5 units over 1 second using Outback ease for a little "pop"
        transform.DOMoveY(transform.position.y + 1f, lifetime).SetEase(Ease.OutBack);
        
        // Fade alpha to 0 over 1 second
        // OnComplete is DOTween's way of executing code when the animation is finished
        textMesh.DOFade(0f, lifetime).OnComplete(() =>
        {
            // Send a signal for FloatingManager to return this object back to the pool
            _returnToPool?.Invoke(this);
        });
    }
}