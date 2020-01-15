using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    public float chunkX = 50;
    public float chunkY = 50;
    public float chunkZ = 0;

    public GameObject ground;
    public GameObject water;
    public List<GameObject> forest;
    public GameObject mountains;
    public GameObject crystal;

    public int NoiseScale = 17;
    public int Seed = 45786;

    // Start is called before the first frame update
    void Start()
    {
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

                /*if (Mathf.PerlinNoise(x * Flatscale, Mathf.PerlinNoise(y, z) * Flatscale) > 0.5)
                {*/
                /*float height = Mathf.PerlinNoise(x, Mathf.PerlinNoise(y, z));
                height = Mathf.Round(height * HeightScale);*/

                Vector3 pos = new Vector3(x, 0, z);
                GameObject newBlock = Instantiate(objectToSpwan, pos, Quaternion.identity);
                newBlock.transform.parent = this.transform;

                //}
            }
        }

        //Lightmapping.Bake();
    }
}
