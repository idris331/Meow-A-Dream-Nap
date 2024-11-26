using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Outline))]
public class InteractableBase : MonoBehaviour
{
    [Space(10)]
    [SerializeField] protected AudioSource _clickCompleteSFX;

    [HideInInspector][SerializeField] protected Transform _transform;

    [HideInInspector][SerializeField] private Outline _outline;

    private Color _normalColor => new Color(0.3686f, 0.5294f, 0.5725f);
    private Color _highlightColor => Color.white;

    [HideInInspector][SerializeField] private NavMeshSurface _navMeshSurface;
    [HideInInspector][SerializeField] protected Camera _mainCamera;

    protected virtual void OnValidate()
    {
        _transform = this.transform;

        _outline = this.GetComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.OutlineColor = _normalColor;
        _outline.OutlineWidth = 5f;

        _navMeshSurface = this.GetComponentInParent<NavMeshSurface>();
        _mainCamera = Camera.main;
    }

    protected virtual void Awake() { }

    public virtual bool canSelect => true;

    public void Select() => _outline.OutlineColor = _highlightColor;
    public void Deselect() => _outline.OutlineColor = _normalColor;

    protected bool _isHeld = false;

    public virtual void Hold() => _isHeld = true;
    public virtual void Release() => _isHeld = false;

    protected virtual void Update()
    {
        _outline.enabled = !PlayerController.isMoving && !AutoMovePlatform.movementAvailable && SceneLoader.loadState == SceneLoader.LoadState.None;
    }

    protected void RebakeNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }
}
