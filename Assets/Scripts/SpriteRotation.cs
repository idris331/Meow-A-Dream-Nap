using UnityEngine;

public class SpriteRotation : MonoBehaviour
{
    [HideInInspector][SerializeField] private Transform _transform;
    [HideInInspector][SerializeField] private Camera _mainCamera;
    [HideInInspector][SerializeField] private Transform _cameraTransform;

    private void OnValidate()
    {
        _transform = this.transform;

        _mainCamera = Camera.main;

        if (_mainCamera == null)
            return;

        _cameraTransform = _mainCamera.transform;
        _transform.forward = GetDirectionToCamera();
    }

    private void Update()
    {
        _transform.forward = GetDirectionToCamera();
    }

    private Vector3 GetDirectionToCamera()
    {
        Vector3 direction = -_cameraTransform.forward;

        return new Vector3(direction.x, 0, direction.z);
    }
}
