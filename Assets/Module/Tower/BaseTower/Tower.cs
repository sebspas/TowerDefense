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
            var newDistance = Vector3.Distance(newCandidate.transform.position, transform.position);
            var selectedDistance = float.MaxValue;
            if (selectedTarget)
            {
                selectedDistance = Vector3.Distance(selectedTarget.transform.position, transform.position);
            }
            
            if (newDistance <= _attackComponent.attackRange && newDistance <= selectedDistance)
            {
                return true;
            }
            return false;
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
