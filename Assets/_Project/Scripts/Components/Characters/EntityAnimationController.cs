using UnityEngine;

public class EntityAnimationController : MonoBehaviour
{
    private Animator _animator;
    private readonly float _moveThreshold = 0.1f;

    private void Awake() => _animator = GetComponent<Animator>();

    public void UpdateAnimation(Vector2 moveInput)
    {
        // Safety Check
        if (_animator == null) return;
        
        bool isWalking = moveInput.magnitude > _moveThreshold;
        _animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            Vector2 snappedDirection = GetSnappedDirection(moveInput);
            
            _animator.SetFloat("InputX", snappedDirection.x);
            _animator.SetFloat("InputY", snappedDirection.y);
            _animator.SetFloat("LastInputX", snappedDirection.x);
            _animator.SetFloat("LastInputY", snappedDirection.y);
        }
    }
    
    public void StopAnimation() 
    {
        _animator.SetBool("IsWalking", false);
    }
    
    public void FaceDirection(Vector2 lookDirection)
    {
        if (_animator == null) return;

        Vector2 snappedDirection = GetSnappedDirection(lookDirection);
        
        _animator.SetFloat("LastInputX", snappedDirection.x);
        _animator.SetFloat("LastInputY", snappedDirection.y);
    }
    
    private Vector2 GetSnappedDirection(Vector2 moveInput)
    {
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            return new Vector2(Mathf.Sign(moveInput.x), 0);
        }
        else
        {
            return new Vector2(0, Mathf.Sign(moveInput.y));
        }
    }
}
