using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot]
public class TileInfo
{
    /*The tile coordinates on the source Tile Sheet in TileMapOrigin that serves as a starting
    point for getting exact pixels*/
    [XmlElement]
    public int tileTextureCoordsX;
    [XmlElement]
    public int tileTextureCoordsY;

    //Bool that determines if this tile is solid or not
    [XmlElement]
    public bool isSolid = false;



    //Default constructor
    public TileInfo()
    {
        this.tileTextureCoordsX = 0;
        this.tileTextureCoordsY = 0;
        this.isSolid = false;
    }

    //Constructor that sets this tile's image. The tile is not solid by default
    public TileInfo(int tileCoordsX_, int tileCoordsY_)
    {
        //Saves the XY coords that get the starting position from our source Tile Sheet
        this.tileTextureCoordsX = tileCoordsX_;
        this.tileTextureCoordsY = tileCoordsY_;

        //Saves this tile's collision type
        this.isSolid = false;
    }


    //Constructor that sets this tile's image and if it's solid or not
    public TileInfo(int tileCoordsX_, int tileCoordsY_, bool solidTile_)
    {
        //Saves the XY coords that get the starting position from our source Tile Sheet
        this.tileTextureCoordsX = tileCoordsX_;
        this.tileTextureCoordsY = tileCoordsY_;

        //Saves this tile's collision type
        this.isSolid = solidTile_;
    }
}