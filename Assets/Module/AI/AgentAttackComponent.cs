using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class AgentAttackComponent : AgentComponent
{
    public float attackRange = 2.0f;
    public float attackDelay = 1.0f;
    public List<IHealthModifier> attackEffect = new List<IHealthModifier>();

    private float _elapsedTime = 0.0f;

    public bool IsTargetInRange()
    {
        HealthComponent targetHealthComponent = _owner.Target.GetComponent<HealthComponent>();
        return Vector3.Distance(_owner.Target.transform.position - targetHealthComponent.attackOffset, transform.position) <= attackRange;
    }

    public void AttackTarget()
    {
        HealthComponent targetHealthComponent = _owner.Target.GetComponent<HealthComponent>();
        if (!targetHealthComponent)
        {
            Debug.Log("Trying to attack a target without a healthComponent.");
        }

        foreach (var modifier in attackEffect)
        {
            targetHealthComponent.AddModifier(modifier);    
        }
    }

    void Start()
    {
        Initialize();
    }

    public void OnUpdate()
    {
        if (_owner.Target)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= attackDelay && IsTargetInRange())
            {
                AttackTarget();
                _elapsedTime = 0.0f;
            }
        }
    }
}
