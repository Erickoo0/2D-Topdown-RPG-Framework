using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.01f;

    private TMP_Text _textComponent;
    private Coroutine _typingCoroutine;
    private string _fullText;

    // Public property so other scripts can check if its currently typing
    public bool IsTyping { get; private set; }

    private void Awake() => _textComponent = GetComponent<TMP_Text>();
    
    public void StartTyping(string textToType)
    {
        _fullText = textToType;
        // Stop the previous coroutine if it's still running'
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        
        // Start a new coroutine
        _typingCoroutine = StartCoroutine(TypeText(_fullText));
    }

    private IEnumerator TypeText(string text)
    {
        IsTyping = true;
        _textComponent.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            _textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        IsTyping = false;
    }

    public void FinishInstantly()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _textComponent.text = _fullText;
        IsTyping = false;
    }
    
}
