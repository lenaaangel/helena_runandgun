using System;
using System.Collections;
using UnityEditor.Rendering.LookDev;
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
    private float _moveDirection;
    private float _lookDirection;

    private float _facingDirection = 1f;

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
       
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    void Start()
    {
        _moveAction.performed += context => OnMovePerformed(_isMoveHeld, context.ReadValue<float>());
        _moveAction.canceled += context => OnMoveCancelled();
        _attackAction.performed += OnAttackPressed;
        _attackAction.canceled += OnAttackCancelled;
        _jumpAction.performed += OnPlayerJump;
        _lookAction.performed += context => OnLookPerformed(_isLookingUp, context.ReadValue<float>());
        _lookAction.canceled += context => OnLookCancelled();
        _playerDeath += OnPlayerDeath;
        _standingColliderSize = _collider.size;
        _standingColliderOffset = _collider.offset;
        
    }

    void Update()
    {
        Debug.Log(_facingDirection);
        ApplyGravity();
    }

    IEnumerator onMoveHeld(float direction)
    {
        while (_isMoveHeld == true)
        {

            _rb.AddForce(new Vector2(direction, 0) * _moveForce, ForceMode2D.Force);
            yield return null;
        }
    }

    void OnMovePerformed(bool _isMoving, float direction)
    {
        if (direction > 0) _facingDirection = 1;
        else if (direction < 0) _facingDirection = -1;
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
            _lookPerformed?.DynamicInvoke(false, direction);
        }
        
    }

    void OnLookCancelled()
    {
        _collider.size = _standingColliderSize;
        _collider.offset = _standingColliderOffset;
        _isLookingDown = false;
        _isLookingUp = false;
        _lookPerformed?.DynamicInvoke(_isLookingUp, 0);
    }

    void OnPlayerJump(InputAction.CallbackContext context)
    {

        if (!_isGrounded || _isLookingDown) return;
        if (context.performed)
        {
            _isGrounded = false;
            PlayerEvents.RaiseJumpPerformed(true);
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) return;
        if (collision.gameObject.CompareTag("Ground") && !_isGrounded)
        {
            _isGrounded = true;
            PlayerEvents.RaiseJumpPerformed(false);

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
            rb.AddForce(new Vector2(_facingDirection, 0f) * 10f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(1f);
        
        }
        
    }

}
