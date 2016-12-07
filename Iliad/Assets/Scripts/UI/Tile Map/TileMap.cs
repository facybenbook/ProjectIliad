using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;


[XmlRoot]
[SerializeField]
public class TileMap
{
    //The texture file that this tile map gets tiles from
    /*[XmlElement]
    public Texture SourceTexture { get; set; }*/

    //The height and width of each tile's image in pixels
    [XmlElement]
    public int TilePixelSize { get; set; }

    //The height and width of each tile on the grid in units
    [XmlElement]
    public float TileGridSize { get; set; }

    //The 2D grid of TileInfo tiles that map up this map
    [XmlElement]
    public TileInfo[,] TileGrid { get; set; }

    //The number of tiles above this map's origin
    [XmlElement]
    public int TilesUp { get; set; }

    //The number of tiles below this map's origin
    [XmlElement]
    public int TilesDown { get; set; }

    //The number of tiles left of this map's origin
    [XmlElement]
    public int TilesLeft { get; set; }

    //The number of tiles right of this map's origin
    [XmlElement]
    public int TilesRight { get; set; }



    //Public constructor for this TileMap class
    public TileMap()
    {
        //The number of pixels wide/high each tile is
        this.TilePixelSize = 32;
        //The width/height of the tiles on the Unity grid
        this.TileGridSize = 1;

        //The number of rows above the origin
        this.TilesUp = 1;
        //The number of rows below the origin
        this.TilesDown = 0;
        //The number of columns right of the origin
        this.TilesRight = 1;
        //The number of columns left of the origin
        this.TilesLeft = 0;

        //Creating a 1x1 2D array of tiles
        this.TileGrid = new TileInfo[1,1]
        {
            { new TileInfo(TestColors.Red) }
        };
    }
}


public enum TestColors
{
    None,
    Red,
    Blue,
    Green
}