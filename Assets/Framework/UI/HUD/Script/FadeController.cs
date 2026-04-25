using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }
    
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float snapThreshold = 0.10f;
    private CanvasGroup _fadeCanvas;

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple FadeControllers detected. Deleting one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _fadeCanvas = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeOut()
    {
        while (_fadeCanvas.alpha < 1 - snapThreshold)
        {
            _fadeCanvas.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
        _fadeCanvas.alpha = 1;
    }

    public IEnumerator FadeIn()
    {
        while (_fadeCanvas.alpha > snapThreshold)
        {
            _fadeCanvas.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        _fadeCanvas.alpha = 0;
    }
}
