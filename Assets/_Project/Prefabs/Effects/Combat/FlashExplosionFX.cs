using UnityEngine;

public class FlashExplosionFX : MonoBehaviour
{
    [SerializeField] private string animationName = "FlashExplosionFX";
    // Radius that matches sprite 
    [SerializeField] private float nativeSpriteRadius = 0.9f;

    private Animator _animator;

    private void Awake() => _animator = GetComponent<Animator>();

    public void SetupExplosion(float gameplayRadius)
    {
        // 1. Calculate explosion scale
        float neededScale = gameplayRadius / nativeSpriteRadius;
        transform.localScale = new Vector3(neededScale, neededScale, 1f);
        
        // 2. Force the animation to play from the beginning
        _animator.Play(animationName, -1, 0f);
    }

    // The animator will call this method on the last frame
    public void DestroySelf() => Destroy(gameObject);
}
