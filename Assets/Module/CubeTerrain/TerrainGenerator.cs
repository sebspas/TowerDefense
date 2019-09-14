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
        /*for (int y = 0; y < chunkY; y++)
        {*/
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

        Lightmapping.Bake();
        //}
    }

    public static float[,,] Calc3D(int width, int height, int length, float scale)
    {
        var values = new float[width, height, length];
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                for (var k = 0; k < length; k++)
                    values[i, j, k] = Generate(i * scale, j * scale, k * scale) * 128 + 128;
        return values;
    }

    private static float Generate(float x, float y, float z)
    {
        // Simple skewing factors for the 3D case
        const float F3 = 0.333333333f;
        const float G3 = 0.166666667f;

        float n0, n1, n2, n3; // Noise contributions from the four corners

        // Skew the input space to determine which simplex cell we're in
        var s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
        var xs = x + s;
        var ys = y + s;
        var zs = z + s;
        var i = FastFloor(xs);
        var j = FastFloor(ys);
        var k = FastFloor(zs);

        var t = (i + j + k) * G3;
        var X0 = i - t; // Unskew the cell origin back to (x,y,z) space
        var Y0 = j - t;
        var Z0 = k - t;
        var x0 = x - X0; // The x,y,z distances from the cell origin
        var y0 = y - Y0;
        var z0 = z - Z0;

        // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
        // Determine which simplex we are in.
        int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
        int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords

        /* This code would benefit from a backport from the GLSL version! */
        if (x0 >= y0)
        {
            if (y0 >= z0)
            { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
            else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
            else { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
        }
        else
        { // x0<y0
            if (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
            else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
            else { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
        }

        // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
        // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
        // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
        // c = 1/6.

        var x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
        var y1 = y0 - j1 + G3;
        var z1 = z0 - k1 + G3;
        var x2 = x0 - i2 + 2.0f * G3; // Offsets for third corner in (x,y,z) coords
        var y2 = y0 - j2 + 2.0f * G3;
        var z2 = z0 - k2 + 2.0f * G3;
        var x3 = x0 - 1.0f + 3.0f * G3; // Offsets for last corner in (x,y,z) coords
        var y3 = y0 - 1.0f + 3.0f * G3;
        var z3 = z0 - 1.0f + 3.0f * G3;

        // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
        var ii = Mod(i, 256);
        var jj = Mod(j, 256);
        var kk = Mod(k, 256);

        // Calculate the contribution from the four corners
        var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
        if (t0 < 0.0f) n0 = 0.0f;
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * Grad(_perm[ii + _perm[jj + _perm[kk]]], x0, y0, z0);
        }

        var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
        if (t1 < 0.0f) n1 = 0.0f;
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1]]], x1, y1, z1);
        }

        var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
        if (t2 < 0.0f) n2 = 0.0f;
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * Grad(_perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2]]], x2, y2, z2);
        }

        var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
        if (t3 < 0.0f) n3 = 0.0f;
        else
        {
            t3 *= t3;
            n3 = t3 * t3 * Grad(_perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1]]], x3, y3, z3);
        }

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to stay just inside [-1,1]
        return 32.0f * (n0 + n1 + n2 + n3); // TODO: The scale factor is preliminary!
    }

    private static int Mod(int x, int m)
    {
        var a = x % m;
        return a < 0 ? a + m : a;
    }

    private static float Grad(int hash, float x, float y, float z)
    {
        var h = hash & 15;     // Convert low 4 bits of hash code into 12 simple
        var u = h < 8 ? x : y; // gradient directions, and compute dot product.
        var v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
        return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
    }

    private static int FastFloor(float x)
    {
        return (x > 0) ? ((int)x) : (((int)x) - 1);
    }

    private static byte[] _perm;
}
