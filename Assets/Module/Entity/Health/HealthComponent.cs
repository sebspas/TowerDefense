using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float max = 100.0f;
    public float current = 0.0f;
    public Vector3 attackOffset = new Vector3(3f,0);

    private List<IHealthModifier> activeModifier = new List<IHealthModifier>();

    public void AddModifier(IHealthModifier modifier)
    {
        modifier.Initialize(this.gameObject);
        activeModifier.Add(modifier);
    }

    void Die()
    {
        Debug.Log("Entity died.");
        Destroy(this);
    }

    void Start()
    {
        current = max;
    }

    void Update()
    {
        //Clean up done modifier
        var list = new List<int>(Enumerable.Range(1, 10));
        for (int i = activeModifier.Count - 1; i >= 0; i--)
        {
            if (activeModifier[i].CurrentState == EState.Done)
                activeModifier.RemoveAt(i);
        }

        // Update active one
        foreach (var modifier in activeModifier)
        {
            modifier.OnUpdate();
        }

        if (current <= 0.0f)
        {
            Die();
        }
    }
}
