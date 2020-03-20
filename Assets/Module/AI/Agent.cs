using System;
using System.Collections.Generic;
using UnityEngine;

public enum EAgentState
{
    Idle,
    Move,
    Attack
}

public class Agent : MonoBehaviour
{    
    private MovementComponent _movementComponent;
    private AttackComponent _attackComponent;
    private TargetFinder _targetFinder;
    private EAgentState _currentAgentState = EAgentState.Idle;
    private Func<GameObject, GameObject, bool> findTargetCondition;

    public void Reset()
    {
        _currentAgentState = EAgentState.Idle;
    }

    private void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _attackComponent = GetComponent<AttackComponent>();
        _targetFinder = GetComponent<TargetFinder>();

        findTargetCondition = (newCandidate, selectedTarget) =>
        {
            var transformPos = Map.Get2DPos(transform.position);
            var candidatePosition = Map.Get2DPos(newCandidate.transform.position);

            var newDistance = Vector2.Distance(candidatePosition, transformPos);
            var selectedDistance = float.MaxValue;
            if (selectedTarget)
            {
                selectedDistance = Vector2.Distance(Map.Get2DPos(selectedTarget.transform.position), transformPos);
            }

            if (newDistance <= selectedDistance && newDistance <= _movementComponent.DistanceToCurrentTarget)
            {
                if (_movementComponent.TryToGetNavigablePositionFromTarget(newCandidate, out var pos))
                {
                    return true;
                }
            }
            return false;
        };
    }

    private void UpdateTarget()
    {
        if (!_targetFinder.FindNewTarget(findTargetCondition)) return;

        _movementComponent.SetDestination(_targetFinder.Target);
        _attackComponent.Target = _targetFinder.Target;
    }

    private void Update()
    {
        UpdateTarget();

        // If no target go back to idle
        if (!_targetFinder.Target)
        {
            _currentAgentState = EAgentState.Idle;
        }

        switch (_currentAgentState)
        {
            case EAgentState.Idle:                
                if (_targetFinder.Target)
                {
                    _currentAgentState = EAgentState.Move;                
                }
                break;
            case EAgentState.Move:
                if (_attackComponent.IsTargetInRange(_targetFinder.Target))
                {
                    _currentAgentState = EAgentState.Attack;
                    _movementComponent.StopNavigation();
                }
                else
                {
                    _movementComponent.OnUpdate();
                } 
                break;
            case EAgentState.Attack:
                if (_attackComponent.IsTargetInRange(_targetFinder.Target))
                {
                    _attackComponent.OnUpdate();
                }
                else
                {
                    _currentAgentState = EAgentState.Move;                   
                }
                break;
        }        
    }
}
