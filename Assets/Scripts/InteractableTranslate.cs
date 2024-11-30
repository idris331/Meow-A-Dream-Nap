using UnityEngine;

public class InteractableTranslate : InteractableBase
{
    [Space(10)]
    [SerializeField] private Transform _endTransform;

    [Space(10)]
    [SerializeField] private InteractableTranslate _connectedTranslate;

    [Space(10)]
    [SerializeField] private InteractableBase _attachechedInteractable;
    private Vector3 _attachedOffset;

    [HideInInspector][SerializeField] private Vector3 _startPoint;
    [HideInInspector][SerializeField] private Vector3 _endPoint;

    [HideInInspector][SerializeField] private Vector3 _translateDirection;
    [HideInInspector][SerializeField] private float _translateDistance;

    private Vector3 _offset;
    private float _zCoordinate;
    private float _translatePercent = 0;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (_endTransform == null)
            return;

        _startPoint = _transform.position;
        _endPoint = _endTransform.position;

        _translateDirection = (_endPoint - _startPoint).normalized;
        _translateDistance = Vector3.Distance(_startPoint, _endPoint);
    }

    protected override void Awake()
    {
        base.Awake();

        if (_attachechedInteractable != null)
        {
            _attachedOffset = _attachechedInteractable.transform.localPosition;

            _attachechedInteractable.transform.parent = _transform.parent;
        }
    }

    public override void Hold()
    {
        base.Hold();

        _zCoordinate = _mainCamera.WorldToScreenPoint(transform.position).z;

        _offset = _transform.position - GetMouseWorldPosition();
    }

    public override void Release()
    {
        base.Release();

        RebakeNavMesh();

        _clickCompleteSFX.Play();
    }

    protected override void Update()
    {
        base.Update();

        if (!_isHeld)
            return;

        Vector3 newPosition = GetMouseWorldPosition() + _offset;

        newPosition.x = Mathf.Clamp(newPosition.x, Mathf.Min(_startPoint.x, _endPoint.x), Mathf.Max(_startPoint.x, _endPoint.x));
        newPosition.y = Mathf.Clamp(newPosition.y, Mathf.Min(_startPoint.y, _endPoint.y), Mathf.Max(_startPoint.y, _endPoint.y));
        newPosition.z = Mathf.Clamp(newPosition.z, Mathf.Min(_startPoint.z, _endPoint.z), Mathf.Max(_startPoint.z, _endPoint.z));

        _transform.position = newPosition;

        CalculateTranslatePercentage();

        if (_attachechedInteractable != null)
            _attachechedInteractable.transform.position = _transform.position + _attachedOffset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = _zCoordinate;

        return _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void CalculateTranslatePercentage()
    {
        float totalDistance = Vector3.Distance(_startPoint, _endPoint);

        float currentDistance = Vector3.Distance(_startPoint, transform.position);

        if (totalDistance > 0)
            _translatePercent = (currentDistance / totalDistance);
        else
            _translatePercent = 0;

        if (_connectedTranslate != null)
            _connectedTranslate.SetTranslatePercent(_translatePercent);
    }

    private void SetTranslatePercent(float percent)
    {
        _translatePercent = percent;

        _transform.position = _startPoint + (_translateDirection * _translateDistance * _translatePercent);
    }
}
