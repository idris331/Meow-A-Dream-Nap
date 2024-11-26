using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    private const string _interactableLayerName = "Interactable";
    public static LayerMask interactableLayer => LayerMask.NameToLayer(_interactableLayerName);

    private InteractableBase[] _interactables;

    private InteractableBase _selectedInteractable;
    private InteractableBase selectedInteractable
    {
        get => _selectedInteractable;

        set
        {
            if (value == _selectedInteractable)
                return;

            if (_selectedInteractable != null)
                _selectedInteractable.Deselect();

            _selectedInteractable = value;

            if (_selectedInteractable != null)
                _selectedInteractable.Select();
        }
    }

    private bool _isHolding = false;

    private void Awake()
    {
        _interactables = FindObjectsOfType<InteractableBase>();
    }

    private void Update() => CheckInteractables();

    private void CheckInteractables()
    {
        if (PlayerController.isMoving || AutoMovePlatform.movementAvailable)
        {
            selectedInteractable = null;
            return;
        }

        if (!_isHolding)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            LayerMask checkLayerMask = (1 << interactableLayer);

            InteractableBase availableInteractable = null;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, checkLayerMask))
            {
                foreach (InteractableBase interactable in _interactables)
                {
                    if (hit.collider.gameObject.transform.parent.gameObject != interactable.gameObject)
                        continue;

                    availableInteractable = interactable;
                    break;
                }
            }

            selectedInteractable = availableInteractable;
        }

        if (selectedInteractable == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            selectedInteractable.Hold();
            _isHolding = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedInteractable.Release();
            _isHolding = false;
        }
    }
}
