using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _sandbag;
    [SerializeField] private Vector3 _followOffset;

    private Vector3 _rightOffset;
    private Vector3 _leftOffset;
    private Transform _cameraTransform;
    private float _lookDirection = 1f;

    void Start()
    {
        _rightOffset = _followOffset;
        _leftOffset = new Vector3(-_followOffset.x, _followOffset.y, -1);
        PlayerEvents.OnMove += OnPlayermove;
        _cameraTransform = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position,_sandbag.position + _followOffset,1f * Time.deltaTime);
    }

    void OnPlayermove(bool isMoving,float direction)
    {
        _lookDirection = direction;
        if(_lookDirection < 0)
        {
            _followOffset = _leftOffset;
        }
        else if(_lookDirection > 0)
        {
            _followOffset = _rightOffset;
        }
    }
}
