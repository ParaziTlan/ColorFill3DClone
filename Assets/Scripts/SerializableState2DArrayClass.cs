using System;
using UnityEngine;
[Serializable]
public class SerializableState2DArrayClass
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private LevelController.TileStates[] values;

    public int Width => width;
    public int Height => height;
    public LevelController.TileStates this[int x, int y]
    {
        get { return values[y * width + x]; }
        set
        {
            if (x < 0 || y < 0) throw new IndexOutOfRangeException("Negatif DEĞER !! Dizide Negatif olmaz!!!");
            if (x >= width || y >= height) throw new IndexOutOfRangeException("Dizi lenghtinden büyük!!!");
            values[y * width + x] = value;
            LevelController.instance.UpdateTileVisuals(x, y, values[y * width + x]);
        }
    }
    public SerializableState2DArrayClass(int width, int height)
    {
        this.width = width;
        this.height = height;
        values = new LevelController.TileStates[width * height];
    }
}
