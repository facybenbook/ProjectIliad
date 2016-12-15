using UnityEngine;
using System;

[Serializable]
public class TileInfo : System.Object
{
    /*The tile coordinates on the source Tile Sheet in TileMapOrigin that serves as a starting
    point for getting exact pixels*/
    [SerializeField]
    public int tileTextureCoordsX;
    [SerializeField]
    public int tileTextureCoordsY;

    //Bool that determines if this tile is solid or not
    [SerializeField]
    public bool isSolid = false;

    //Testing enum that should be removed
    [SerializeField]
    public TestColors tileTestColor = TestColors.None;



    //Default constructor
    public TileInfo()
    {
        this.tileTextureCoordsX = -1;
        this.tileTextureCoordsY = -1;
        this.isSolid = false;
    }

    //Test constructor
    public TileInfo(TestColors color)
    {
        this.tileTextureCoordsX = -1;
        this.tileTextureCoordsY = -1;
        this.isSolid = false;

        tileTestColor = color;
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