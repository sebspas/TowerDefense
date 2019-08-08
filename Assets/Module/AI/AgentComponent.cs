using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class AgentComponent : MonoBehaviour
{
    protected Agent _owner;

    protected void Initialize()
    {
        _owner = GetComponent<Agent>();
    }
}
