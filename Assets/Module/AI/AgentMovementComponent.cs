using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentMovementComponent : AgentComponent
{
    public float delayToReCalculatePath = 1.0f;
    
    public float DistanceToCurrentTarget => _distanceToCurrentTarget;

    private float _distanceToCurrentTarget = float.MaxValue;
    private NavMeshAgent _navMeshAgent;
    private float _elapsedTimeToRecalculatePath = 0.0f;

    public void SetDestination(Vector3 newDestination)
    {

        HealthComponent targetHealthComponent = _owner.Target.GetComponent<HealthComponent>();

        _navMeshAgent.SetDestination(newDestination - targetHealthComponent.attackOffset);
        _distanceToCurrentTarget = Vector3.Distance(transform.position, newDestination);
        _navMeshAgent.isStopped = false;
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
        if (_owner.Target)
        {
            _elapsedTimeToRecalculatePath += Time.deltaTime;
            if (_elapsedTimeToRecalculatePath >= delayToReCalculatePath)
            {
                _elapsedTimeToRecalculatePath = 0.0f;
                SetDestination(_owner.Target.transform.position);
            }
        }
    }

    private void Start()
    {
        Initialize();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
}
