using System;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManageInput : MonoBehaviour
{
    [Header("Movement Values")]
    [SerializeField] private float _moveForce = 5f;

    private bool _isMoveHeld;
    private bool _isAttacking;
    private bool _isLooking;
    private float _moveDirection;
    private float _lookDirection;

    private InputSystem_Actions _inputActions;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private Action<bool, float> _movePerformed;
    private Action<bool, float> _lookPerformed;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _moveAction = _inputActions.Player.Move;
        _lookAction = _inputActions.Player.Look;
        _jumpAction = _inputActions.Player.Jump;
        _attackAction = _inputActions.Player.Attack;
        _movePerformed = PlayerEvents.RaiseMovePerformed;
        _lookPerformed = PlayerEvents.RaiseLookPerformed;
        _rb = GetComponent<Rigidbody2D>();
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
        _lookAction.performed += context => OnLookPerformed(_isLooking, context.ReadValue<float>());
        _lookAction.canceled += context => OnLookCancelled();    

    }

    void Update()
    {
        Debug.Log(_lookDirection);
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
            PlayerEvents.RaisePlayerAttack(_isAttacking);
        }
    }

    void OnAttackCancelled(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            _isAttacking = false;
            PlayerEvents.RaisePlayerAttack(_isAttacking);
        }
    }

    void OnLookPerformed(bool isLooking, float direction)
    {
        if (direction < 0) return;
        _isLooking = true;
        isLooking = _isLooking;
        _lookDirection = direction;
        _lookPerformed?.DynamicInvoke(_isLooking, _lookDirection);
    }

    void OnLookCancelled()
    {
        _isLooking = false;
        _lookPerformed?.DynamicInvoke(_isLooking, 0);
    }
}
