using UnityEngine;

[System.Serializable]
public class Tile
{
    public int x;
    public int z;

    public TileType type;

    // Item that could be on this tile (building, tower...)
    public GameObject item;

    public Tile(int x, int z, TileType type)
    {
        this.x = x;
        this.z = z;
        this.type = type;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(x, z);
    }

    public Vector3 Get3DPosition()
    {
        return new Vector3(x, 0.0f, z);
    }

    public override string ToString()
    {
        return "[x:" + x + ";" + "z:" + z + "]";
    }
}
