using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class TileMap : System.Object
{
    //The height and width of each tile's image in pixels
    [SerializeField]
    public int TilePixelSize;

    //The 2D grid of TileInfo tiles that map up this map
    [SerializeField]
    public List<TileInfo> TileGrid;

    //The number of tiles above this map's origin
    [SerializeField]
    public int TilesUp;

    //The number of tiles below this map's origin
    [SerializeField]
    public int TilesDown;

    //The number of tiles left of this map's origin
    [SerializeField]
    public int TilesLeft;

    //The number of tiles right of this map's origin
    [SerializeField]
    public int TilesRight;



    //Public constructor for this TileMap class
    public TileMap()
    {
        //The number of pixels wide/high each tile is
        this.TilePixelSize = 32;

        //The number of rows above the origin
        this.TilesUp = 1;
        //The number of rows below the origin
        this.TilesDown = 1;
        //The number of columns right of the origin
        this.TilesRight = 1;
        //The number of columns left of the origin
        this.TilesLeft = 1;

        //Creating a 1x1 2D array of tiles
        this.TileGrid = new List<TileInfo>()
        {
            new TileInfo(TestColors.None),
            new TileInfo(TestColors.None),
            new TileInfo(TestColors.None),
            new TileInfo(TestColors.None)
        };
    }
}


public enum TestColors
{
    None,
    Red,
    Blue,
    Green,
    Yellow
}