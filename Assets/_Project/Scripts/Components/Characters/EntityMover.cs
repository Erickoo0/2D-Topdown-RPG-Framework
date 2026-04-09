using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMover : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D _rigidbody;
    private Vector2 _moveDirection;
    private bool _canMove = true;
    
    public Vector2 MoveDirection => _moveDirection;
    
    private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (PauseManager.IsGamePaused)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (_canMove)
        {
            _rigidbody.linearVelocity = _moveDirection * (moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void SetMOveDirection(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (force > 0 && gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockbackRoutine(direction, force, duration));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {
        _canMove = false;
        
        // Apply physical impulse
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.AddForce(direction * (force * _rigidbody.mass), ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(duration);
        
        // Restore control
        _rigidbody.linearVelocity = Vector2.zero;
        _canMove = true;
    }
}
