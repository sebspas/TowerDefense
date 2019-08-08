using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    public GameObject Target => _target;
    public List<string> targetListByPriority = new List<string>();

    private GameObject _target = null;

    // Return true if the target has been found and it''s different from the actual one
    public bool FindNewTarget(Func<GameObject, GameObject, bool> condition)
    {
        var prevTarget = _target;
        foreach (var targetTag in targetListByPriority)
        {
            var potentialTarget = GameObject.FindGameObjectsWithTag(targetTag);

            foreach (var potGameObject in potentialTarget)
            {
                if (condition(potGameObject, _target))
                {
                    _target = potGameObject;                    
                }                
            }
        }

        return prevTarget != _target ? _target : false;
    }
}
