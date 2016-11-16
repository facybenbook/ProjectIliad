using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;


[XmlRoot]
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
    public List<List<TileInfo>> TileGrid { get; set; }

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
        this.TilePixelSize = 1;
        this.TileGridSize = 1;

        //Creating a 1x1 2D array of tiles
        this.TileGrid = new List<List<TileInfo>>(1)
        {
            new List<TileInfo>(0)
        };

        this.TilesUp = 0;
        this.TilesDown = 0;
        this.TilesRight = 0;
        this.TilesLeft = 0;
    }
}