using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMover : MonoBehaviour
{
    [Header("Movement Settings")] 
    public float moveSpeed = 5f;
    [SerializeField] private float knockbackDecay = 5f;
    private bool _isKnockedBack = false;
    private float _knockbackTimer;
    
    private Rigidbody2D _rigidbody;
    private Vector2 _moveDirection;
    
    public Vector2 MoveDirection => _moveDirection;
    
    private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (PauseManager.IsGamePaused)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (_isKnockedBack)
        {
            // When knocked back, actively apply friction to slow down speed
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.linearVelocity = Vector2.Lerp(_rigidbody.linearVelocity, Vector2.zero, knockbackDecay *  Time.fixedDeltaTime); 
            
            // Snap velocity when close to zero
            if (_rigidbody.linearVelocity.sqrMagnitude < 0.01f) 
            {
                _rigidbody.linearVelocity = Vector2.zero;
                _isKnockedBack = false; // Give control back immediately!
            }
            else
            {
                // Fallback Timer (in case they are pushed against a treadmill or moving platform)
                _knockbackTimer -= Time.fixedDeltaTime;
                if (_knockbackTimer <= 0f)
                {
                    _isKnockedBack = false;
                    _rigidbody.linearVelocity = Vector2.zero; 
                }
            }
        }
        else
        {
            // If no movement input and not knockedback, lock their position to prevent being pushed around
            if (_moveDirection == Vector2.zero)
            {
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                _rigidbody.linearVelocity = Vector2.zero;
            }
            else
            {
                // Normal movement  (unlock position)
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                _rigidbody.linearVelocity = _moveDirection * moveSpeed;
            }
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (force > 0 && gameObject.activeInHierarchy)
        {
            _isKnockedBack = true;
            _knockbackTimer = duration;
            // Send the entity knocked back
            _rigidbody.linearVelocity = direction * force;
        }
    }
}
