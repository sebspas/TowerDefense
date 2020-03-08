using System;

[Serializable]
public class MapArea
{
    public float startValue = 0.10f;
    public float endValue = 0.15f;

    public TileType type;

    private static uint _counter = 0;
    private uint id;

    public MapArea()
    {
        id = ++_counter;
    }
}