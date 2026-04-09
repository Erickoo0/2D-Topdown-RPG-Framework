using UnityEngine;

[RequireComponent(typeof(Animator), (typeof(EntityMover)))]
public class EntityAnimator : MonoBehaviour
{
    private Animator _animator;
    private EntityMover _entityMover;

    [Header("Animation Settings")] 
    [Tooltip("It true, snaps animations to 4 way cardinal directions")]
    [SerializeField] private bool snapToCardinalDirections;
    
    private readonly float _moveThreshold = 0.25f;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _entityMover = GetComponent<EntityMover>();
    }

    private void Update()
    {
        // Read the direction from the EntityMover
        Vector2 moveDirection = _entityMover.MoveDirection;
        
        // Only set walking if move is significant
        bool isWalking = moveDirection.magnitude > _moveThreshold; 
        _animator.SetBool("IsWalking", isWalking);
        
        if (isWalking)
        {
            Vector2 animationDirection = snapToCardinalDirections ? GetSnappedDirection(moveDirection) : moveDirection;
            
            _animator.SetFloat("InputX", animationDirection.x);
            _animator.SetFloat("InputY", animationDirection.y);
            
            // Store the last facing direction for idle animations
            _animator.SetFloat("LastInputX", animationDirection.x);
            _animator.SetFloat("LastInputY", animationDirection.y);
        }
    }

    /// <summary>
    /// Forces the entity to look in a specific direction without input / walking
    /// Useful for dialogue interactions, cutscenes, etc
    /// </summary>
    public void FaceDirection(Vector2 lookDirection)
    {
        // Safety check
        if (_animator == null || lookDirection == Vector2.zero) return;
        
        Vector2 animDir = snapToCardinalDirections ? GetSnappedDirection(lookDirection) : lookDirection;

        _animator.SetFloat("LastInputX", animDir.x);
        _animator.SetFloat("LastInputY", animDir.y);
    }

    private Vector2 GetSnappedDirection(Vector2 moveInput)
    {
        // Favors horizontal animation if diagonal input is perfectly equal
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
