using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public float attackRange = 2.0f;
    public float attackDelay = 1.0f;    

    protected GameObject _target;

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
        return Vector2.Distance(target.transform.position - targetHealthComponent.attackOffset, transform.position) <= attackRange;
    }

    public virtual void AttackTarget() {}

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
        else
        {
            _elapsedTime = 0.0f;
        }
    }
}
