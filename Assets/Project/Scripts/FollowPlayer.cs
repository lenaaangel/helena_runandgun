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

    // Update is called once per frame
    void Update()
    {
        _cameraTransform.position = _sandbag.position + _followOffset;
    }
}
