using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    public float chunkX = 50;
    public float chunkZ = 50;
    public float chunkOffset = 10;

    public Mesh mesh;

    public GameObject ground;
    public GameObject water;
    public List<GameObject> forest;
    public GameObject mountains;
    public GameObject crystal;

    public int NoiseScale = 17;
    public int Seed = 45786;

    public void GenerateTerrain()
    {
        float offsetedChunkZ = chunkZ + chunkOffset;
        float offsetedChunkX = chunkX + chunkOffset;

        GeneratePerlinNoiseMap(offsetedChunkZ, offsetedChunkX);

        CreateNavMesh(new Vector3(10, 1, 10), new Vector3(offsetedChunkX / 2, 0.5f, offsetedChunkZ / 2), new Vector3(0, 0, 0));
    }

    public Vector3 GetMapCenter()
    {
        return new Vector3(chunkX/2, 0, chunkZ/2);
    }

    void GeneratePerlinNoiseMap(float chunkZ, float chunkX)
    {
        Debug.Log("Start Map Generation");

        for (var z = 0; z < chunkZ; z++)
        {
            for (var x = 0; x < chunkX; x++)
            {
                //Terrain algorithm
                float perlinNoise = Mathf.PerlinNoise((x / chunkX * NoiseScale) + Seed, (z / chunkZ * NoiseScale) + Seed);

                GameObject objectToSpwan = null;
                if (perlinNoise <= 0.33)
                {
                    int treeType = Mathf.RoundToInt(Random.Range(0, forest.Count));

                    objectToSpwan = forest[treeType];
                }
                else if (perlinNoise > 0.33 && perlinNoise <= 0.66)
                {
                    objectToSpwan = ground;
                }
                else if (perlinNoise > 0.66 && perlinNoise <= 0.67)
                {
                    objectToSpwan = crystal;
                }
                else
                {
                    objectToSpwan = mountains;
                }

                Vector3 pos = new Vector3(x, 0, z);
                GameObject newBlock = Instantiate(objectToSpwan, pos, Quaternion.identity);
                newBlock.transform.parent = this.transform;
            }
        }

        Debug.Log("End Map Generation");
    }

    void CreateNavMesh(Vector3 scale, Vector3 pos, Vector3 rotation)
    {
        Debug.Log("Creating NavMesh");

        GameObject planeGO = new GameObject("plane");
        MeshFilter mf = planeGO.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        planeGO.transform.position = pos;
        planeGO.transform.Rotate(rotation);
        planeGO.transform.localScale = scale;

        NavMeshSurface navMeshSurface = planeGO.AddComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();

        Debug.Log("NavMesh Created!");
    }
}
