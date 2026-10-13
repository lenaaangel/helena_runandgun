using Unity.VisualScripting;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Vector2 _standingLeftOffset;
    [SerializeField] private Vector2 _standingRightOffset;
    [SerializeField] private Vector2 _crouchingOffset;
    [SerializeField] private Vector2 _lookingUpOffset;
    [SerializeField] private Vector2 _lookingDownOffset;
    

    private bool _lookingLeft;
    private bool _isJumping;
    private float _lookDirection;


    private void Awake()
    {
        PlayerEvents.OnMove += OnPlayerMove;
        PlayerEvents.OnLook += OnPlayerLook;
        PlayerEvents.OnJump += OnPlayerJump;
    }

    private void Update()
    {
        //Debug.Log(_isJumping);
    }

    void OnPlayerMove(bool isMoving, float direction)
    {
        if (_isJumping && _lookDirection < 0) return;
        if(direction < 0)
        {
            _lookingLeft = true;
        }else if (direction > 0)
        {
            _lookingLeft = false;
        }

        UpdateMuzzle();
    }

    void OnPlayerLook(bool isMoving, float direction)
    {
        _lookDirection = direction;
        UpdateMuzzle();
    }

    void OnPlayerJump(bool isJumping)
    {
        _isJumping = isJumping;
        UpdateMuzzle();
    }

    void UpdateMuzzle()
    {
        if (_muzzle == null) return;
        Vector2 offset = _lookingLeft ? _standingLeftOffset : _standingRightOffset;
        

        if (_lookDirection < 0)
        {
            if(_isJumping) offset = _lookingDownOffset;
            else offset += _crouchingOffset;

        }else if(_lookDirection > 0)
        {
            offset = _lookingUpOffset;
        }
       
        _muzzle.localPosition = offset;

    }
}
