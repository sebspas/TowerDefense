using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public float attackRange = 2.0f;
    public float attackDelay = 1.0f;

    public float rotateSpeed = 1.0f;

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
        return Vector2.Distance(
            Map.Get2DPos(target.transform.position - targetHealthComponent.attackOffset),
            Map.Get2DPos(transform.position)) <= attackRange;
    }

    public virtual void AttackTarget() {}
    public virtual void RotateTowardTarget(GameObject target) {}

    public void OnUpdate()
    {
        _elapsedTime += Time.deltaTime;

        if (_target)
        {
            RotateTowardTarget(_target);

            if (_elapsedTime >= attackDelay)
            {
                AttackTarget();
                _elapsedTime = 0.0f;
            }
        }
    }
}
