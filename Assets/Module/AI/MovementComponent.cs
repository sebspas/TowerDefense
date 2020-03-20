using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementComponent : MonoBehaviour
{
    public float delayToReCalculatePath = 1.0f;
    
    public float DistanceToCurrentTarget => _distanceToCurrentTarget;

    private float _distanceToCurrentTarget = float.MaxValue;
    private NavMeshAgent _navMeshAgent;
    private float _elapsedTimeToRecalculatePath = 0.0f;
    private GameObject _target;

    public void SetDestination(GameObject target)
    {
        _target = target;
        var targetHealthComponent = target.GetComponent<HealthComponent>();

        if (TryToGetNavigablePositionFromTarget(target, out var targetPos))
        {
            var destination = targetPos - targetHealthComponent.attackOffset;
            _navMeshAgent.SetDestination(destination);
            _distanceToCurrentTarget = Vector3.Distance(transform.position, destination);
            _navMeshAgent.isStopped = false;
        }
        else
        {
            Debug.Log("Cannot navigate to target...");
        }
    }

    public bool TryToGetNavigablePositionFromTarget(GameObject target, out Vector3 outPosition)
    {
        if (CanNavigateTo(target.transform.position))
        {
            outPosition = target.transform.position;
            return true;
        }

        // Try neighboor tile to see if one is navigable
        var newCandidateTile = Map.Instance.GetTileUnderposition(target.transform.position);
        var neighBors = Map.Instance.GetNeighbors(newCandidateTile);

        foreach (var tile in neighBors)
        {
            if (CanNavigateTo(tile.Get3DPosition()))
            {
                outPosition = target.transform.position;
                return true;
            }
        }

        outPosition = new Vector3();
        return false;
    }

    public bool CanNavigateTo(Vector3 position)
    {
        var path = new NavMeshPath();
        _navMeshAgent.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    public void StopNavigation()
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
    }

    public void OnUpdate()
    {
        _elapsedTimeToRecalculatePath += Time.deltaTime;
        if (_elapsedTimeToRecalculatePath >= delayToReCalculatePath)
        {
            _elapsedTimeToRecalculatePath = 0.0f;
            SetDestination(_target);
        }
    }

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
}
