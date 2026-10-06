using Unity.VisualScripting;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Vector2 _standingLeftOffset;
    [SerializeField] private Vector2 _standingRightOffset;
    [SerializeField] private Vector2 _crouchingOffset;
    

    private bool _lookingLeft;
    private float _lookDirection;

    private void Awake()
    {
        PlayerEvents.OnMove += OnPlayerMove;
        PlayerEvents.OnLook += OnPlayerLook;
    }

    private void Update()
    {
        Debug.Log(_lookDirection);
    }

    void OnPlayerMove(bool isMoving, float direction)
    {
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

    void UpdateMuzzle()
    {
        Vector2 offset;
        if (_lookDirection < 0)
        {
            offset = _crouchingOffset;
        }
        else offset = _standingLeftOffset;


            offset = _standingRightOffset;
        if (_lookingLeft)
        {
            offset.x = -_standingLeftOffset.x;
        }
        else
        {
            offset.x = _standingRightOffset.x;
        }
            _muzzle.localPosition = offset;

    }
}
