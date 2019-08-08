using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public float attackRange = 2.0f;
    public float attackDelay = 1.0f;
    public List<IHealthModifier> attackEffect = new List<IHealthModifier>();

    private GameObject _target;

    public GameObject Target
    {
        get => _target;
        set => _target = value;
    }

    private float _elapsedTime = 0.0f;

    public bool IsTargetInRange(GameObject target)
    {
        if (!target) return false;

        HealthComponent targetHealthComponent = target.GetComponent<HealthComponent>();     
        return Vector3.Distance(target.transform.position - targetHealthComponent.attackOffset, transform.position) <= attackRange;
    }

    public void AttackTarget()
    {
        HealthComponent targetHealthComponent = _target.GetComponent<HealthComponent>();
        if (!targetHealthComponent)
        {
            Debug.Log("Trying to attack a target without a healthComponent.");
        }

        foreach (var modifier in attackEffect)
        {
            targetHealthComponent.AddModifier(modifier);    
        }
    }

    public void OnUpdate()
    {
        if (_target)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= attackDelay)
            {
                AttackTarget();
                _elapsedTime = 0.0f;
            }
        }
    }
}
