using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HealthModifier/Damage", order = 1)]
public class DamageModifier : IHealthModifier
{
    public float damage = 1.0f;

    public override void OnUpdate()
    {
        owner.current -= damage;

        currentState = EState.Done;
    }
}
