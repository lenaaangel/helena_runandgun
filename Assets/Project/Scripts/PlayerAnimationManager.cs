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


    void Start()
    {
        PlayerEvents.OnMove += OnPlayerMove;
        PlayerEvents.onAttack += OnPlayerAttack;
        PlayerEvents.OnLook += OnPlayerLook;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlayerMove(bool isMoving, float direction)
    {
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
        
        
        if(_lookDirection < 0)
        {
            _bottomHalfRenderer.enabled = false;
            _topHalfAnimator.SetBool("isCrouching", true);
            _topHalfAnimator.SetBool("isAttacking", isAttacking);
        }
        _topHalfAnimator.SetBool("isAttacking", isAttacking);
        _bottomHalfAnimator.SetBool("isAttacking", isAttacking);
        
    }

    void OnPlayerLook(bool isLooking, float direction)
    {
        _lookDirection = direction;
        if (direction > 0)
        {
            _topHalfAnimator.SetBool("isLooking", isLooking);

        }
        else if (direction < 0)
        {
            _bottomHalfRenderer.enabled = false;
            _topHalfAnimator.SetBool("isCrouching", true);
        }
        else
        {
            _bottomHalfRenderer.enabled = true;
            _topHalfAnimator.SetBool("isCrouching", false);
            _topHalfAnimator.SetBool("isLooking", false);
        }

    }
}
