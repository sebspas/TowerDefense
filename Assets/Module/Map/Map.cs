using System;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private Tile[,] _mapTiles;
    public Tile[,] MapTiles
    {
        get => _mapTiles;
        set => _mapTiles = value;
    }

    private uint _mapSizeZ;
    public uint MapSizeZ
    {
        get => _mapSizeZ;
        set => _mapSizeZ = value;
    }

    private uint _mapSizeX;
    public uint MapSizeX
    {
        get => _mapSizeX;
        set => _mapSizeX = value;
    }

    /*
     * Singleton Implementation
     */
    private static Map instance = null;

    private Map()
    {
    }

    public static Map Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Map();
            }

            return instance;
        }
    }

    /*
     * Helper method
     */
    public Vector3 GetMapCenter()
    {
        return new Vector3((float)_mapSizeX / 2, 0, (float)_mapSizeZ / 2);
    }

    public Tile GetTileUnderposition(Vector3 positiontarget)
    {
        var x = (uint)Math.Floor(positiontarget.x);
        var z = (uint)Math.Floor(positiontarget.z);

        if (x > MapSizeX || z > MapSizeZ)
        {
            return null;
        }
        else
        {
            return _mapTiles[x, z];
        }
    }

    public IEnumerable<Tile> GetNeighbors(Tile tile)
    {
        var neighbors = new List<Tile>();

        // Left
        if (tile.x - 1 >= 0)
        {
            neighbors.Add(_mapTiles[tile.x - 1, tile.z]);
        }
        // Right
        if (tile.x + 1 < Map.Instance.MapSizeX)
        {
            neighbors.Add(_mapTiles[tile.x + 1, tile.z]);
        }
        // UP
        if (tile.z + 1 < Map.Instance.MapSizeZ)
        {
            neighbors.Add(_mapTiles[tile.x, tile.z + 1]);
        }
        // DOWN
        if (tile.z - 1 >= 0)
        {
            neighbors.Add(_mapTiles[tile.x, tile.z - 1]);
        }

        return neighbors;
    }

    public static Vector2 Get2DPos(Vector3 position)
    {
        return new Vector2(position.x, position.z);
    }
}
