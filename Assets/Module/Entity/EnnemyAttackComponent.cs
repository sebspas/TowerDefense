using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAttackComponent : AttackComponent
{
    public List<IHealthModifier> attackEffect = new List<IHealthModifier>();

    public override void AttackTarget()
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
}
