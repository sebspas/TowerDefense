using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType", order = 2)]
[System.Serializable]
public class TileType : ScriptableObject
{
    public string tileName = "Unnamed_Tile";
    public GameObject prefab;
    public bool IsWalkable = false;
}
