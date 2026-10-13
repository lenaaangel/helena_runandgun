using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [Header("Player Animators")]
    [SerializeField] private GameObject _topHalf;
    [SerializeField] private Animator _topHalfAnimator;
    [SerializeField] private Animator _bottomHalfAnimator;
    [SerializeField] private SpriteRenderer _topHalfRenderer;
    [SerializeField] private SpriteRenderer _bottomHalfRenderer;

    private float _lookDirection;
    private bool _isJumping;
    private bool _isDead;

    private void OnEnable()
    {
        PlayerEvents.OnMove += OnPlayerMove;
        PlayerEvents.onAttack += OnPlayerAttack;
        PlayerEvents.OnLook += OnPlayerLook;
        PlayerEvents.OnJump += OnPlayerJump;
        PlayerEvents.onDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerEvents.OnMove -= OnPlayerMove;
        PlayerEvents.onAttack -= OnPlayerAttack;
        PlayerEvents.OnLook -= OnPlayerLook;
        PlayerEvents.OnJump -= OnPlayerJump;
        PlayerEvents.onDeath -= OnPlayerDeath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlayerMove(bool isMoving, float direction)
    {
        if (_isDead) return;

        _topHalfAnimator.SetBool("isMoving", isMoving);
        _bottomHalfAnimator.SetBool("isMoving", isMoving);
        if(direction > 0)
        {
            _topHalfRenderer.flipX = false;
            _bottomHalfRenderer.flipX = false;
        }
        else if(direction < 0)
        {
            _topHalfRenderer.flipX = true;
            _bottomHalfRenderer.flipX = true;
        }
    }

    void OnPlayerAttack(bool isAttacking) 
    {
        
        if (_isDead) return;

        if (_lookDirection < 0)
        {
            if(!_isJumping) _bottomHalfRenderer.enabled = false;
            _topHalfAnimator.SetBool("isCrouching", true);
            _topHalfAnimator.SetBool("isAttacking", isAttacking);
        }
        _topHalfAnimator.SetBool("isAttacking", isAttacking);
        _bottomHalfAnimator.SetBool("isAttacking", isAttacking);
        
    }

    void OnPlayerLook(bool isLooking, float direction)
    {
        if (_isDead) return;
        _lookDirection = direction;
        if (direction > 0)
        {
            _topHalfAnimator.SetBool("isLooking", isLooking);

        }
        else if (direction < 0)
        {
            if (!_isJumping) _bottomHalfRenderer.enabled = false;
            _topHalfAnimator.SetBool("isCrouching", true);
        }
        else
        {
            _bottomHalfRenderer.enabled = true;
            _topHalfAnimator.SetBool("isCrouching", false);
            _topHalfAnimator.SetBool("isLooking", false);
        }

    }

    void OnPlayerJump(bool isJumping)
    {
        if (_isDead) return;
        _isJumping = isJumping;
        _bottomHalfAnimator.SetBool("isJumping", isJumping);
        _topHalfAnimator.SetBool("isJumping", isJumping);
    }

    void OnPlayerDeath(bool isDead)
    {
        _isDead = isDead;
        _topHalfRenderer.enabled = false;
        _bottomHalfAnimator.SetBool("isMoving", false);
        _bottomHalfAnimator.SetBool("isAttacking", false);
        _bottomHalfAnimator.SetBool("isJumping", false);
        _bottomHalfAnimator.SetBool("isDead", isDead);
    }
}
