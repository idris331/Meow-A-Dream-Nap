using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector][SerializeField] private Transform _transform;

    [HideInInspector][SerializeField] private Animator _animator;

    private const string _directionXAnim = "directionX";
    private const string _directionZAnim = "directionZ";

    private const string _isMovingAnim = "isMoving";

    private const string _goSleepAnim = "goSleep";

    [HideInInspector][SerializeField] private PlayerSFX _playerSFX;

    [HideInInspector][SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private Transform[] _goals;

    private int _currentGoalIndex = 0;

    public static bool isMoving { get; private set; } = false;

    private bool _isWaiting = false;

    public delegate void GoalEvent();
    public static event GoalEvent onReachLastGoal;

    private void OnValidate()
    {
        _transform = this.transform;
        _animator = this.GetComponentInChildren<Animator>();
        _playerSFX = this.GetComponentInChildren<PlayerSFX>();
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    private void OnAwake() => SetFacing(_transform.forward);

    private void OnDisable()
    {
        StopAllCoroutines();

        isMoving = false;
    }

    private void Update() => CheckGoals();
    
    private void CheckGoals()
    {
        if (_currentGoalIndex >= _goals.Length)
            return;

        if (!isMoving)
        {
            Vector3 currentGoal = _goals[_currentGoalIndex].position;

            bool canReachGoal = CanReachPoint(currentGoal);

            if (canReachGoal && !AutoMovePlatform.movementAvailable)
            {
                _navMeshAgent.SetDestination(currentGoal);
                
                isMoving = true;

                _animator.SetBool(_isMovingAnim, true);
                _playerSFX.PlayFootStep(true);
            }
        }
        else
        {
            if (_isWaiting)
                return;

            if (_navMeshAgent.remainingDistance <= 0)
            {
                _currentGoalIndex++;

                if (_currentGoalIndex >= _goals.Length)
                    onReachLastGoal?.Invoke();

                _animator.SetBool(_isMovingAnim, false);

                _playerSFX.PlayFootStep(false);

                _isWaiting = true;

                StartCoroutine(DelayToStopMoving());
            }
            else
                SetFacing(_navMeshAgent.velocity.normalized);
        }
    }

    private IEnumerator DelayToStopMoving()
    {
        yield return new WaitForSeconds(0.1f);

        isMoving = false;
        _isWaiting = false;
    }

    private void SetFacing(Vector3 directionFacing)
    {
        if (directionFacing == Vector3.zero)
            return;

        directionFacing.y = 0;

        _transform.forward = directionFacing;

        if (Mathf.Abs(directionFacing.x) > Mathf.Abs(directionFacing.z))
        {
            _animator.SetFloat(_directionXAnim, Mathf.Sign(directionFacing.x));
            _animator.SetFloat(_directionZAnim, 0);
        }
        else
        {
            _animator.SetFloat(_directionXAnim, 0);
            _animator.SetFloat(_directionZAnim, Mathf.Sign(directionFacing.z));
        }
    }

    private bool CanReachPoint(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(_navMeshAgent.transform.position, targetPosition, NavMesh.AllAreas, path))
            return path.status == NavMeshPathStatus.PathComplete;

        return false;
    }
}
