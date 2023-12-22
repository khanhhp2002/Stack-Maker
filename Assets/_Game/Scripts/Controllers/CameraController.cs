using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _speed;
    private Transform _playerTransform;
    private Vector3 _defaultPosition;
    void Start()
    {
        _defaultPosition = transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(_playerTransform is not null)
        {
            transform.position = Vector3.Lerp(transform.position, _playerTransform.position + _offset, Time.fixedDeltaTime * _speed);
        }
        else
        {
             transform.position = Vector3.Lerp(transform.position, _defaultPosition + _offset, Time.fixedDeltaTime * _speed);
        }
    }

    public void SetTarget(PlayerController target)
    {
        _playerTransform = target?.transform;
    }
}
