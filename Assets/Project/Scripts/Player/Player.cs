using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Values")]
    [SerializeField] private float _moveForce = 5f;
    [SerializeField] private float _jumpForce = 2f;
    [Header("Collider Values")]
    [SerializeField] private Vector2 _colliderCrouchingOffset;
    [SerializeField] private Vector2 _colliderCrouchingSize;
    [Header("Bullet Prefab")][SerializeField] private GameObject _bulletPrefab;
    [Header("Weapon Muzzle")][SerializeField] private Transform _muzzle;


    private Vector2 _standingColliderSize;
    private Vector2 _standingColliderOffset;

    private bool _isMoveHeld;
    private bool _isAttacking;
    private bool _isLookingUp;
    private bool _isLookingDown;
    private bool _isGrounded;
    private bool _isDead;
    //private bool _canJump;
    private float _moveDirection;
    private float _lookDirection;
    
    //private float _coyoteTimer;
    //private float _coyoteTime = 5f;

    private Vector2 _facingDirection = new Vector2(1,0);
    private Vector2 _lastFacingDirection;

    private InputSystem_Actions _inputActions;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private Action<bool, float> _movePerformed;
    private Action<bool, float> _lookPerformed;
    private Action<bool> _playerDeath;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _collider;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _moveAction = _inputActions.Player.Move;
        _lookAction = _inputActions.Player.Look;
        _jumpAction = _inputActions.Player.Jump;
        _attackAction = _inputActions.Player.Attack;
        _movePerformed = PlayerEvents.RaiseMovePerformed;
        _lookPerformed = PlayerEvents.RaiseLookPerformed;
        _playerDeath = PlayerEvents.RaisePlayerDeath;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
    }


    private void OnEnable()
    {
        _inputActions.Enable();
        _moveAction.performed += context => OnMovePerformed(_isMoveHeld, context.ReadValue<float>());
        _moveAction.canceled += context => OnMoveCancelled();
        _attackAction.performed += OnAttackPressed;
        _attackAction.canceled += OnAttackCancelled;
        _jumpAction.performed += OnPlayerJump;
        _lookAction.performed += context => OnLookPerformed(_isLookingUp, context.ReadValue<float>());
        _lookAction.canceled += context => OnLookCancelled();
        _playerDeath += OnPlayerDeath;
        PlayerEvents.onGamePaused += OnGamePaused;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        _moveAction.performed -= context => OnMovePerformed(_isMoveHeld, context.ReadValue<float>());
        _moveAction.canceled -= context => OnMoveCancelled();
        _attackAction.performed -= OnAttackPressed;
        _attackAction.canceled -= OnAttackCancelled;
        _jumpAction.performed -= OnPlayerJump;
        _lookAction.performed -= context => OnLookPerformed(_isLookingUp, context.ReadValue<float>());
        _lookAction.canceled -= context => OnLookCancelled();
        _playerDeath -= OnPlayerDeath;
        PlayerEvents.onGamePaused -= OnGamePaused;
    }

    void Start()
    {
        //_coyoteTimer = _coyoteTime;
        _lastFacingDirection = _facingDirection;
        _standingColliderSize = _collider.size;
        _standingColliderOffset = _collider.offset;
        
    }



    void Update()
    {
        //Debug.Log(_coyoteTimer);
        //Debug.Log(_canJump);
        ApplyGravity();
    }

    IEnumerator onMoveHeld(float direction)
    {
        while (_isMoveHeld == true)
        {

            _rb.AddForce(new Vector2(direction, 0) * (_moveForce), ForceMode2D.Force);
            yield return null;
        }
    }

    void OnMovePerformed(bool _isMoving, float direction)
    {
        if (direction > 0) _facingDirection = new Vector2(1,0);
        else if (direction < 0) _facingDirection = new Vector2(-1, 0);
        _lastFacingDirection = _facingDirection;
        if (_isLookingDown || _isLookingDown && _isAttacking) return;
        _isMoveHeld = true;
        _isMoving = _isMoveHeld;
        _movePerformed?.DynamicInvoke(_isMoving, direction);
        StartCoroutine(onMoveHeld(direction));
    }

    void OnMoveCancelled() 
    {
        _isMoveHeld = false;
        _movePerformed?.Invoke(_isMoveHeld, 0f);
    }

    void OnAttackPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            _isAttacking = true;
            StartCoroutine(Shoot());
            PlayerEvents.RaisePlayerAttack(_isAttacking);
        }
    }

    void OnAttackCancelled(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            _isAttacking = false;
            StartCoroutine(Shoot());
            PlayerEvents.RaisePlayerAttack(_isAttacking);
        }
    }

    void OnLookPerformed(bool isLooking, float direction)
    {
        if (direction > 0) 
        {
            _facingDirection = new Vector2(0,1f);
            _isLookingUp = true;
            isLooking = _isLookingUp;
            _lookDirection = direction;
            _lookPerformed?.DynamicInvoke(_isLookingUp, _lookDirection);
        }else if(direction < 0)
        {
            if(_isMoveHeld) return;
            _collider.offset += _colliderCrouchingOffset;
            _collider.size += _colliderCrouchingSize;
            _isLookingDown = true;
            if(!_isGrounded) _facingDirection = new Vector2(0, -1f);
            _lookPerformed?.DynamicInvoke(false, direction);
        }
        
        
    }

    void OnLookCancelled()
    {
        _collider.size = _standingColliderSize;
        _collider.offset = _standingColliderOffset;
        _isLookingDown = false;
        _isLookingUp = false;
        _facingDirection = _lastFacingDirection;
        _lookPerformed?.DynamicInvoke(_isLookingUp, 0);
    }

    void OnPlayerJump(InputAction.CallbackContext context)
    {
        //if (!_canJump) return;
        if (!_isGrounded || _isLookingDown) return;
        if (context.performed)
        {
            _isGrounded = false;
            //_canJump = false;
            PlayerEvents.RaiseJumpPerformed(true);
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) return;
        if (collision.gameObject.CompareTag("Ground") && !_isGrounded)
        {
            //_coyoteTimer = _coyoteTime;
            _isGrounded = true;
            //_canJump = true;
            if(!_isLookingUp)_facingDirection = _lastFacingDirection;
            PlayerEvents.RaiseJumpPerformed(false);
            //StopCoroutine(CoyoteTime());
        }
        if (_isDead)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _rb.bodyType = RigidbodyType2D.Static;
                _collider.enabled = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
            //StartCoroutine(CoyoteTime());
        }
    }

    void ApplyGravity()
    {
        if(_isGrounded)
        {
            _rb.gravityScale = 1f;
        }
        else
        {
            _rb.gravityScale = 5f;
        }
    }

    public void OnPlayerDeath(bool isDead)
    {
        _isDead = isDead;
        PlayerEvents.RaisePlayerDeath(_isDead);
        _inputActions.Disable();
        
    }

    IEnumerator Shoot()
    {
        
        while(_isAttacking && !_isDead)
        {
            GameObject bullet = Instantiate(_bulletPrefab, new Vector3(_muzzle.position.x, _muzzle.position.y, 0), Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(_facingDirection * 20f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
        }
        
    }

   /* private IEnumerator CoyoteTime()
    {
        if (!_isGrounded)
        {
            while (_coyoteTimer > 0)
            {
                _canJump = true;
                _coyoteTimer = _coyoteTimer - Time.deltaTime;
                yield return null;
            }
            _canJump = false;
            yield return null;
        }
    }
   */
    void OnGamePaused(bool isGamePaused)
    {
        if (isGamePaused) _inputActions.Disable();
        if (!isGamePaused) _inputActions.Enable();
    }

}
