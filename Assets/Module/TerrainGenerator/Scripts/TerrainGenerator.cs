using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public uint chunkX = 50;
    public uint chunkZ = 50;
    public uint chunkOffset = 10;

    [Header("Spawn properties")]
    public uint spawnSize = 5;
    public TileType spawnAreaType;
    public TileType debugType;
    public GameObject border;

    [Header("Noise properties")]
    public uint noiseScale = 17;
    public float seed = 45786;

    [Header("Areas")]
    public List<MapArea> areaToGenerate;

    // Internal map data
    private Tile[,] _map;
    private uint _mapSizeZ;
    private uint _mapSizeX;

    class TileCluster
    {
        public TileCluster()
        {
            border = new List<Tile>();
            tiles = new List<Tile>();
        }

        public readonly List<Tile> border;
        public readonly List<Tile> tiles;
    }

    public void GenerateTerrain()
    {
        _mapSizeX = chunkX;
        _mapSizeZ = chunkZ;
        _map = new Tile[_mapSizeX, _mapSizeZ];

        // Generate a first map from raw perlin noise
        GeneratePerlinNoiseMap();

        // Add A center area to spawn the player
        CreateCenterSpawn();

        // Make sure all the area are linked together
        LinkAllArea();

        // Place Map Border
        PlaceMapBorder();

        // Add ressources and collectible on the map
        // TODO

        // Finally instantiate the Object
        InstantiateMapGameObject();

        // Create one navmesh for the allMap
        // TODO change this to calculate the pathfinding myself using the map data
        CreateNavMesh(new Vector3((chunkX) / 10, 1, (chunkZ) / 10),
            new Vector3(_mapSizeX / 2, 0.5f, _mapSizeX / 2),
            new Vector3(0, 0, 0));
    }

    private void PlaceMapBorder()
    {
        // left 
        var wall = Instantiate(border, new Vector3(-1, 0, (float)(_mapSizeZ-1)/ 2), Quaternion.identity);
        wall.transform.localScale = new Vector3(1,1, _mapSizeZ);
        // top
        wall = Instantiate(border, new Vector3((float)(_mapSizeX - 1) / 2, 0, _mapSizeZ), Quaternion.identity);
        wall.transform.localScale = new Vector3(_mapSizeX,1, 1);
        // right
        wall = Instantiate(border, new Vector3(_mapSizeX, 0, (float)(_mapSizeZ - 1) / 2), Quaternion.identity);
        wall.transform.localScale = new Vector3(1,1, _mapSizeZ);
        // bot
        wall = Instantiate(border, new Vector3((float)(_mapSizeX - 1) / 2, 0, -1), Quaternion.identity);
        wall.transform.localScale = new Vector3(_mapSizeX, 1, 1);
    }

    public Vector3 GetMapCenter()
    {
        return new Vector3(chunkX/2, 0, chunkZ/2);
    }

    private MapArea GetMapAreaFromPerlinNoise(float perlinValue)
    {
        foreach (var area in areaToGenerate)
        {
            if (perlinValue >= area.startValue && perlinValue <= area.endValue)
            {
                return area;
            }
        }

        Debug.LogWarning($"The perlin noise value {perlinValue} is not covered by your data.");
        return null;
    }

    /// <summary>
    /// Draw a square area to spawn the player and make sure he doesn't spawn in a mountain or a tree
    /// </summary>
    private void CreateCenterSpawn()
    {
        var centerPos = GetMapCenter();

        var minX = (uint)centerPos.x - (spawnSize / 2);
        var minZ = (uint)centerPos.z - (spawnSize / 2);

        for (var x = minX; x < (uint)centerPos.x + (spawnSize / 2); x++)
        {
            for (var z = minZ; z < (uint)centerPos.z + (spawnSize / 2); z++)
            {
                _map[x, z].type = spawnAreaType;
            }
        }
    }

    private void GeneratePerlinNoiseMap()
    {
        Debug.Log("Start Map Generation");

        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                //Terrain algorithm
                var perlinNoise = Mathf.Abs(Mathf.PerlinNoise(
                    ((float)x / _mapSizeZ * noiseScale) + seed,
                    ((float)z / _mapSizeX * noiseScale) + seed));

                var areaToSpawn = GetMapAreaFromPerlinNoise(perlinNoise);

                _map[x,z] = new Tile(x, z, areaToSpawn.type);
            }
        }

        Debug.Log("End Map Generation");
    }

    private void InstantiateMapGameObject()
    {
        for (var x = 0; x < _mapSizeX; x++)
        {
            for (var z = 0; z < _mapSizeZ; z++)
            {
                var tile = _map[x, z];
                var pos = new Vector3(x, 0, z);                
                var newBlock = Instantiate(tile.type.prefab, pos, Quaternion.identity);
                newBlock.transform.parent = transform;
                newBlock.name = tile.type.name + x + ":" + z;
            }
        }
    }

    private void CreateNavMesh(Vector3 scale, Vector3 pos, Vector3 rotation)
    {
        Debug.Log("Creating NavMesh");

        var planeGo = new GameObject("plane");

        planeGo.transform.position = pos;
        planeGo.transform.Rotate(rotation);
        planeGo.transform.localScale = scale;

        var navMeshSurface = planeGo.AddComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();

        Debug.Log("NavMesh Created!");
    }

    private void LinkAllArea()
    {
        List<TileCluster> listOfCluster = new List<TileCluster>();

        // First pass on the map to find floor tile
        for (var x = 0; x < _mapSizeX; x++)
        {
            for (var z = 0; z < _mapSizeZ; z++)
            {
                var tile = _map[x, z];
                // When we find one expand it to create a cluster if not already inside one
                if (tile.type.IsWalkable)
                {
                    var found = false;
                    foreach (var cluster in listOfCluster)
                    {
                        if (cluster.tiles.Contains(tile))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        // Create a new cluster from this tile
                        TileCluster newCluster = CreateNewClusterFromTile(tile);
                        UpdateClusterBorder(newCluster);
                        listOfCluster.Add(newCluster);

                    }
                }
            }
        }

        Debug.Log($"Number of cluster to merge is {listOfCluster.Count}");

        while (listOfCluster.Count != 1)
        {
            MergeTwoClosestCluster(listOfCluster);
        }
    }

    private void UpdateClusterBorder(TileCluster newCluster)
    {
        newCluster.border.Clear();
        foreach (var tile in newCluster.tiles)
        {
            if (IsBorder(tile))
            {
                newCluster.border.Add(tile);
            }
        }
    }

    private bool IsBorder(Tile tile)
    {
        foreach (var neighbor in GetNeighbors(tile))
        {
            if (!neighbor.type.IsWalkable)
            {
                return true;
            }
        }

        return false;
    }

    private TileCluster CreateNewClusterFromTile(Tile tile)
    {
        var newCluster = new TileCluster();

        var visitedTiles = new List<Tile>();
        var toExplore = new Queue<Tile>();

        newCluster.tiles.Add(tile);
        toExplore.Enqueue(tile);

        while (toExplore.Count > 0)
        {
            var current = toExplore.Dequeue();
            foreach (var neighbor in GetNeighbors(current))
            {
                if (neighbor.type.IsWalkable && !visitedTiles.Contains(neighbor))
                {
                    visitedTiles.Add(neighbor);
                    newCluster.tiles.Add(neighbor);
                    toExplore.Enqueue(neighbor);
                }
            }
        }

        return newCluster;
    }

    private IEnumerable<Tile> GetNeighbors(Tile tile)
    {
        var neighbors = new List<Tile>();

        // Left
        if (tile.x - 1 >= 0)
        {
            neighbors.Add(_map[tile.x - 1, tile.z]);
        }
        // Right
        if (tile.x + 1 < _mapSizeX)
        {
            neighbors.Add(_map[tile.x + 1, tile.z]);
        }
        // UP
        if (tile.z + 1 < _mapSizeZ)
        {
            neighbors.Add(_map[tile.x, tile.z + 1]);
        }
        // DOWN
        if (tile.z - 1 >= 0)
        {
            neighbors.Add(_map[tile.x, tile.z - 1]);
        }

        return neighbors;
    }

    private void MergeTwoClosestCluster(IList<TileCluster> clusterList)
    {
        // Find the twos tiles that are the closest
        var minDistance = uint.MaxValue;
        var cluster1 = clusterList[0];
        var cluster2 = clusterList[1];

        Tile t1 = null, t2 = null;

        // TODO optimize later
        
        for (int i = 1; i < clusterList.Count; i++)
        {
            var currentCluster2 = clusterList[i];
            foreach (var tile1 in cluster1.border)
            {
                foreach (var tile2 in cluster2.border)
                {
                    if (tile1 == tile2) continue;

                    var dt1t2 = DistanceManhattan(tile1, tile2);
                    if (dt1t2 >= minDistance) continue;

                    minDistance = dt1t2;
                    t1 = tile1;
                    t2 = tile2;
                    cluster2 = currentCluster2;
                }
            }
        }

        // Draw a line between them
        CreateBasicPathBetweenTile(t1, t2);

        // Merge the cluster2 with cluster1
        cluster1.tiles.InsertRange(cluster1.tiles.Count, cluster2.tiles);
        UpdateClusterBorder(cluster1);
        clusterList.Remove(cluster2);
    }

    private static uint DistanceManhattan(Tile t1, Tile t2)
    {
        return (uint)(Mathf.Abs((float)t2.x - t1.x) + Mathf.Abs((float)t2.z - t1.z));
    }

    private void CreateBasicPathBetweenTile(Tile source, Tile dest)
    {
        Debug.Log($"Creating a path between two tiles {source},{dest}");
        var currentX = source.x;
        var currentZ = source.z;

        while (currentX != dest.x)
        {
            if (currentX > dest.x)
            {
                currentX--;
            }
            else
            {
                currentX++;
            }

            _map[currentX, currentZ].type = debugType;
        }

        while (currentZ != dest.z)
        {
            if (currentZ > dest.z)
            {
                currentZ--;
            }
            else
            {
                currentZ++;
            }

            _map[currentX, currentZ].type = debugType;
        }
    }
}
