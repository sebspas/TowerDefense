using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Building", order = 3)]
[Serializable]
public class Building : ScriptableObject
{
    public String test = "test";

    public GameObject Preview;

    public GameObject BuildingPrefab;
}
