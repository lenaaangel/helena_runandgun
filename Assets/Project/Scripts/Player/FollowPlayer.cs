using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _sandbag;
    [SerializeField] private Vector3 _followOffset;
    private Transform _cameraTransform;

    void Start()
    {
        _cameraTransform = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position,_sandbag.position + _followOffset,1f * Time.deltaTime);
    }
}
