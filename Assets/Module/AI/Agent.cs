using System;
using System.Collections.Generic;
using UnityEngine;

public enum EAgentState
{
    Idle,
    Move,
    Attack
}

[RequireComponent(typeof(AgentMovementComponent))]
[RequireComponent(typeof(AgentAttackComponent))]
public class Agent : MonoBehaviour
{    
    public List<string> targetListByPriority = new List<string>(); 
    
    private GameObject _target;

    public GameObject Target => _target;

    private AgentMovementComponent _agentMovementComponent;
    private AgentAttackComponent _agentAttackComponent;
    private EAgentState _currentAgentState = EAgentState.Idle;

    private bool UpdateTarget()
    {
        GameObject newTarget = _target;
        foreach (var targetTag in targetListByPriority)
        {
            var potentialTarget = GameObject.FindGameObjectsWithTag(targetTag);
            var prevDistance = float.MaxValue;

            foreach (var potGameObject in potentialTarget)
            {
                var newDistance = Vector3.Distance(potGameObject.transform.position, transform.position);
                if (newDistance <= prevDistance && newDistance <= _agentMovementComponent.DistanceToCurrentTarget)
                {                    
                    if (_agentMovementComponent.CanNavigateTo(potGameObject.transform.position))
                    {
                        prevDistance = newDistance;
                        newTarget = potGameObject;
                    }
                }
            }

            if (newTarget != _target)
            {
                _target = newTarget;
                return true;
            }
        }

        return false;
    }

    private void Start()
    {
        _agentMovementComponent = GetComponent<AgentMovementComponent>();
        _agentAttackComponent = GetComponent<AgentAttackComponent>();
    }

    private void Update()
    {
        switch (_currentAgentState)
        {
            case EAgentState.Idle:
                UpdateTarget();
                _agentMovementComponent.SetDestination(_target.transform.position);
                _currentAgentState = EAgentState.Move;
                break;
            case EAgentState.Move:
                if (_agentAttackComponent.IsTargetInRange())
                {
                    _currentAgentState = EAgentState.Attack;
                    _agentMovementComponent.StopNavigation();
                }
                else
                {
                    UpdateTarget();
                    _agentMovementComponent.OnUpdate();
                }                
                break;
            case EAgentState.Attack:
                if (_agentAttackComponent.IsTargetInRange())
                {
                    _agentAttackComponent.OnUpdate();
                }
                else
                {
                    _currentAgentState = EAgentState.Move;
                    UpdateTarget();
                    _agentMovementComponent.SetDestination(_target.transform.position);
                }
                break;
        }        
    }
}
