using System;
using UnityEngine;

public class FlashShader : MonoBehaviour
{
    [SerializeField] private float flashDecaySpeed = 5f;
    
    private SpriteRenderer[] _spriteRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    private float _flashFactor; // From materiall property block
    
    private void Start()
    {
        // Get all sprite renders from game object
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (_flashFactor <= 0f) return;
        
        _flashFactor = Mathf.Lerp(_flashFactor, 0f, flashDecaySpeed * Time.deltaTime);
        if (_flashFactor < 0.01f)
        {
            _flashFactor = 0f;
        }
        ApplyFlashFactor();
    }

    public void ApplyFlash()
    {
        _flashFactor = 1f;
        ApplyFlashFactor();
    }
    
    private void ApplyFlashFactor()
    {
        foreach (var spriteRenderer in _spriteRenderer)
        {
            // Set the flash factor in the material property block
            spriteRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat("_FlashFactor", _flashFactor);
            spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}
