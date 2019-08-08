using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EState
{
    Starting,
    Running,
    Done
}

public class IHealthModifier : ScriptableObject
{
    protected EState currentState = EState.Starting;

    public EState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }

    protected HealthComponent owner = null;

    public virtual void Initialize(GameObject owner)
    {
        this.owner = owner.GetComponent<HealthComponent>();

        if (!this.owner)
        {
            Debug.Log("Missing HealthComponent on entity.");
        }

        currentState = EState.Running;
    }

    public virtual void OnUpdate() {}
    public virtual void Destruct() {}
}
