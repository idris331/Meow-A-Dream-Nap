using UnityEngine;

public class InteractableRotate : InteractableBase
{
    [Space(10)]
    [SerializeField] private bool _lockX;
    [SerializeField] private bool _lockY;
    [SerializeField] private bool _lockZ;

    private const float _rotateSpeed = 10f;
    private const float _adjustSpeed = 10f;

    private enum RotateAxis { X, Y, Z }
    private RotateAxis? _rotationAxis;

    private enum RotationState { None, AxisSelection, Dragging, Adjusting }
    private RotationState _rotationState = RotationState.None;

    private Quaternion _targetRotation;


    public override bool canSelect => base.canSelect && _rotationState == RotationState.None;

    public override void Hold()
    {
        base.Hold();

        _rotationState = RotationState.AxisSelection;
    }

    public override void Release()
    {
        base.Release();

        switch (_rotationState)
        {
            case RotationState.AxisSelection:

                _rotationState = RotationState.None;

                break;

            case RotationState.Dragging:

                _rotationState = RotationState.Adjusting;
                _rotationAxis = null;

                Vector3 targetEuler = new Vector3(FindClosestAngle(_transform.eulerAngles.x), FindClosestAngle(_transform.eulerAngles.y), FindClosestAngle(_transform.eulerAngles.z));

                _targetRotation = Quaternion.Euler(targetEuler);

                break;
        }
    }

    protected override void Update()
    {
        base.Update();

        switch (_rotationState)
        {
            case RotationState.AxisSelection:

                _rotationAxis = GetAxisToRotate();

                if (_rotationAxis.HasValue)
                {
                    _rotationState = RotationState.Dragging;
                    return;
                }

                break;

            case RotationState.Dragging:

                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                switch (_rotationAxis.Value)
                {
                    case RotateAxis.X:
                        _transform.Rotate(Vector3.right, mouseY * _rotateSpeed, Space.World);
                        break;

                    case RotateAxis.Y:
                        _transform.Rotate(Vector3.up, -mouseX * _rotateSpeed, Space.World);
                        break;

                    case RotateAxis.Z:
                        _transform.Rotate(Vector3.forward, -mouseY * _rotateSpeed, Space.World);
                        break;
                }

                break;

            case RotationState.Adjusting:

                _transform.rotation = Quaternion.Lerp(_transform.rotation, _targetRotation, _adjustSpeed * Time.deltaTime);

                float angleDifference = Quaternion.Angle(_transform.rotation, _targetRotation);

                if (angleDifference <= 10f)
                {
                    _rotationState = RotationState.None;
                    _transform.rotation = _targetRotation;
                    RebakeNavMesh();

                    _clickCompleteSFX.Play();

                    return;
                }

                break;
        }
    }

    private RotateAxis? GetAxisToRotate()
    {
        float mouseMagnitudeX = Mathf.Abs(Input.GetAxis("Mouse X"));
        float mouseMagnitudeY = Mathf.Abs(Input.GetAxis("Mouse Y"));

        if (mouseMagnitudeX < 0.1f && mouseMagnitudeY < 0.1f)
            return null;

        if (mouseMagnitudeX > mouseMagnitudeY)
        {
            if (_lockY)
                return null;

            return RotateAxis.Y;            
        }

        Vector3 cursorScreenPosition = Input.mousePosition;
        cursorScreenPosition.z = _mainCamera.nearClipPlane;

        Vector3 cursorWorldPosition = _mainCamera.ScreenToWorldPoint(cursorScreenPosition);
        cursorWorldPosition = new Vector3(cursorWorldPosition.x, _transform.position.y, cursorWorldPosition.z);

        Vector3 directionToCursor = (cursorWorldPosition - _transform.position).normalized;

        if (directionToCursor.x > directionToCursor.z)
        {
            if (_lockX)
                return null;

            return RotateAxis.X;
        }
        else
        {
            if (_lockZ)
                return null;

            return RotateAxis.Z;
        }
    }

    private float FindClosestAngle(float angle)
    {
        angle = (angle % 360 + 360) % 360;

        float closestAngle = Mathf.Round(angle / 90) * 90;

        return closestAngle;
    }
}
