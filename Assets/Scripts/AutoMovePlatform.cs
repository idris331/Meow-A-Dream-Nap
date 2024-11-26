using UnityEngine;

public class AutoMovePlatform : MonoBehaviour
{
    [SerializeField] private Transform _endTransform;
    [SerializeField] private float _moveDuration;

    private Vector3 _endPoint;
    private float _moveSpeed;

    [HideInInspector][SerializeField] private Transform _transform;
    private bool _moving = false;

    public static bool movementAvailable = false;

    private void OnValidate() => _transform = this.transform;

    private void Awake()
    {
        _endPoint = _endTransform.position;
        _moveSpeed = Vector3.Distance(_transform.position, _endPoint) / _moveDuration;
    }

    private void OnDisable()
    {
        movementAvailable = false;
    }

    private void Update()
    {
        if (_moving)
        {
            _transform.position = Vector3.Lerp(_transform.position, _endPoint, _moveSpeed * Time.deltaTime);
            movementAvailable = true;

            if (Vector3.Distance(_transform.position, _endPoint) < 0.1f)
            {
                _transform.position = _endPoint;
                movementAvailable = false;
                _moving = false;
            }
        }
    }

    public void TriggerMove() => _moving = true;
}
