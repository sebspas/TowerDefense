using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ETowerState
{
    Idle,
    Attack
}

public class Tower : MonoBehaviour
{
    private TargetFinder _targetFinder;
    private Func<GameObject, GameObject, bool> findTargetCondition;
    private AttackComponent _attackComponent;
    private ETowerState _currentAgentState = ETowerState.Idle;

    private void Start()
    {
        _attackComponent = GetComponent<AttackComponent>();
        _targetFinder = GetComponent<TargetFinder>();

        findTargetCondition = (newCandidate, selectedTarget) =>
        {
            Vector2 transformPos = Map.Get2DPos(transform.position);
            Vector2 candidatePosition = Map.Get2DPos(newCandidate.transform.position);

            var newDistance = Vector2.Distance(candidatePosition, transformPos);
            var selectedDistance = float.MaxValue;
            if (selectedTarget)
            {
                selectedDistance = Vector3.Distance(Map.Get2DPos(selectedTarget.transform.position), transformPos);
            }

            return _attackComponent.IsTargetInRange(newCandidate) && newDistance <= selectedDistance;
        };
    }

    private void UpdateTarget()
    {
        if (!_targetFinder.FindNewTarget(findTargetCondition)) return;

        _attackComponent.Target = _targetFinder.Target;
    }

    private void Update()
    {
        UpdateTarget();

        switch (_currentAgentState)
        {
            case ETowerState.Idle:                
                if (_targetFinder.Target)
                {
                    _currentAgentState = ETowerState.Attack;
                }
                break;
            case ETowerState.Attack:
                if (_attackComponent.IsTargetInRange(_targetFinder.Target))
                {
                    _attackComponent.OnUpdate();
                }
                else
                {
                    _currentAgentState = ETowerState.Idle;                
                }
                break;
        }
    }
}
