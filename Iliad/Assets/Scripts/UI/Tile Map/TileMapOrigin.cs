using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[RequireComponent(typeof(SpriteRenderer))]
public class TileMapOrigin : MonoBehaviour
{
    //The file path that holds this tile map's XML data file
    public TextAsset xmlFile;

    //The source image that this tile map uses to texture each tile
    [HideInInspector]
    public Texture sourceTileSheet = null;

    //The width and height of each tile in pixel size
    [HideInInspector]
    public int pixelWidth = 32;

    //The width and height of each tile on Unity's grid
    [HideInInspector]
    public float tileSize = 1;

    //Number of tiles to the left of the origin
    [HideInInspector]
    public int tilesLeft = 0;
    //Number of tiles to the right of the origin
    [HideInInspector]
    public int tilesRight = 0;
    //Number of tiles above the origin
    [HideInInspector]
    public int tilesUp = 0;
    //Number of tiles below the origin
    [HideInInspector]
    public int tilesDown = 0;

    //The 2D list that contains every individual tile for this map
    private List<List<TileInfo>> tileGrid = new List<List<TileInfo>>();
    //2D list that contains all collision verteces for each tile in this tile map
    private List<List<Vector3>> colliderVerts;
    



    //Function called from TileMapEditor when this tile map's grid is changed. Determines whether IncreaseGrid or DecreaseGrid are called
    public int DetermineGridChange(int newValue_ = 0, Directions changeDirection_ = Directions.None)
    {
        //Making sure that this tile grid is initialized
        if (this.tileGrid.Count == 0)
        {
            this.tileGrid = new List<List<TileInfo>>();

            //Loops through to add as many columns as we need
            for(int w = 0; w < (this.tilesLeft + this.tilesRight); ++w)
            {
                List<TileInfo> newListToAdd = new List<TileInfo>();

                //Loops through each column to add as many rows as we need
                for(int h = 0; h < (this.tilesUp + this.tilesDown); ++h)
                {
                    newListToAdd.Add(new TileInfo());
                }

                this.tileGrid.Add(newListToAdd);
            }
        }

        //If no value is added, no function is called
        if (newValue_ < 0 || changeDirection_ == Directions.None)
        {
            return 0;
        }

        //Created an int to hold the difference between the new value and the current
        int difference = 0;
        int current = 0;
        switch(changeDirection_)
        {
            case Directions.Up:
                current = this.tilesUp;
                break;

            case Directions.Down:
                current = this.tilesDown;
                break;

            case Directions.Left:
                current = this.tilesLeft;
                break;

            case Directions.Right:
                current = this.tilesRight;
                break;
        }

        //Finds the total number of rows/columns to be added or subtracted
        difference = newValue_ - current;

        //If the new value is 0, we know we have to subtract all tiles from this direction
        if(newValue_ == 0)
        {
            this.DecreaseGrid(changeDirection_, current);

            return newValue_;
        }

        //If we're decreasing tiles
        if(difference < 0)
        {
            //Making sure we don't subtract more tiles than are there
            if(Mathf.Abs(difference) > current)
            {
                difference = -current;
            }

            this.DecreaseGrid(changeDirection_, Mathf.Abs(difference));
        }
        //If we're increasing tiles
        else
        {
            //Changes this tile map to add some rows or columns
            this.IncreaseGrid(changeDirection_, difference);
        }
        
        //Return the value given, because the TileMapEditor still needs it
        return newValue_;
    }


    //Function that increases the tile grid in the direction given
    public void IncreaseGrid(Directions direction_ = Directions.Right, int numToAdd_ = 1)
    {
        //Does nothing if the number to add isn't a positive number
        if (numToAdd_ < 1)
            return;
        
        //Inserts new rows at the beginning of the first list
        if (direction_ == Directions.Up)
        {
            this.tilesUp += numToAdd_;

            //Loops through a number of times equal to the num to add
            for (int n = 0; n < numToAdd_; ++n)
            {
                this.tileGrid.Insert(0, new List<TileInfo>(this.tilesLeft + this.tilesRight));
            }
        }
        //Adds new rows at the end of the first list
        else if (direction_ == Directions.Down)
        {
            this.tilesDown += numToAdd_;

            //Loops through a number of times equal to the num to add
            for (int n = 0; n < numToAdd_; ++n)
            {
                this.tileGrid.Add(new List<TileInfo>(this.tilesLeft + this.tilesRight));
            }
        }
        //Loops through each row in the first list and inserts new columns at the beginning of the inner lists
        else if (direction_ == Directions.Left)
        {
            this.tilesLeft += numToAdd_;

            //Loops through each row
            for (int r = 0; r < this.tileGrid.Count; ++r)
            {
                for (int n = 0; n < numToAdd_; ++n)
                {
                    this.tileGrid[r].Insert(0, new TileInfo());
                }
            }
        }
        //Loops through each row in the first list and inserts new columns at the end of the inner lists
        else if (direction_ == Directions.Right)
        {
            this.tilesRight += numToAdd_;

            //Loops through each row
            for (int r = 0; r < this.tileGrid.Count; ++r)
            {
                for (int n = 0; n < numToAdd_; ++n)
                {
                    this.tileGrid[r].Add(new TileInfo());
                }
            }
        }

        //Repaints the this tile map's texture
        this.PaintTexture();
        Debug.Log("Increase Grid End. Grid Size: " + this.tileGrid.Count + ", " + this.tileGrid[0].Count);
    }


    //Function that decreases the tile grid in the direction given
    public void DecreaseGrid(Directions direction_ = Directions.Right, int numToRemove_ = 1)
    {
        Debug.Log("Decrease Grid Start. Grid Size: " + this.tileGrid.Count + ", " + this.tileGrid[0].Count);
        //Does nothing if the number to remove isn't a positive number
        if (numToRemove_ < 1)
            return;
        
        int tilesRemoved = numToRemove_;

        //Removes rows at the end of the first list
        if (direction_ == Directions.Up)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tilesUp)
            {
                tilesRemoved = this.tilesUp;
            }

            this.tilesUp -= tilesRemoved;

            //Loops through a number of times equal to the rows removed
            for (int n = 0; n < tilesRemoved; ++n)
            {
                //Nulls and destroys each tile in the removed row
                for (int r = 0; r < this.tileGrid[0].Count; ++r)
                {
                    this.tileGrid[0][r] = null;
                }

                this.tileGrid.RemoveAt(0);
            }
        }
        //Removes rows at the end of the first list
        else if (direction_ == Directions.Down)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tilesDown)
            {
                tilesRemoved = this.tilesDown;
            }

            this.tilesDown -= tilesRemoved;
            
            //Loops through a number of times equal to the rows removed
            for (int n = 0; n < tilesRemoved; ++n)
            {
                //Nulls and destroys each tile in the removed row
                for (int r = 0; r < this.tileGrid[0].Count; ++r)
                {
                    this.tileGrid[this.tileGrid.Count - 1][r] = null;
                }

                this.tileGrid.RemoveAt(this.tileGrid.Count - 1);
            }
        }
        //Loops through each row in the first list and removes columns at the beginning of the inner lists
        else if (direction_ == Directions.Left)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tilesLeft)
            {
                tilesRemoved = this.tilesLeft;
            }

            this.tilesLeft -= tilesRemoved;

            //Loops through each row
            for (int r = 0; r < this.tileGrid.Count; ++r)
            {
                //Destroys, nulls, and removes the first tile in each row
                for (int n = 0; n < tilesRemoved; ++n)
                {
                    this.tileGrid[r][0] = null;
                    this.tileGrid[r].RemoveAt(0);
                }
            }
        }
        else if (direction_ == Directions.Right)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tilesRight)
            {
                tilesRemoved = this.tilesRight;
            }

            this.tilesRight -= tilesRemoved;

            //Loops through each row
            for (int r = 0; r < this.tileGrid.Count; ++r)
            {
                //Destroys, nulls, and removes the last tile in each row
                for (int n = 0; n < tilesRemoved; ++n)
                {
                    this.tileGrid[r][this.tileGrid.Count - 1] = null;
                    this.tileGrid[r].RemoveAt(this.tileGrid.Count - 1);
                }
            }
        }

        //Repaints the this tile map's texture
        this.PaintTexture();

        Debug.Log("Decrease Grid End");
    }


    //Function that sets a tile at a given location in space. If tileToSet_ is NULL, deletes a tile
    public void SetTile(Vector3 clickPos_, TileInfo tileToSet_ = null)
    {
        //Finds the position clicked in local space relative to this tile map
        Vector3 localPos = this.transform.InverseTransformPoint(clickPos_);

        //Now we round the local X position to the nearest grid location
        float roundedXCoord = localPos.x;
        if (roundedXCoord >= 0)
        {
            roundedXCoord = Mathf.Ceil(roundedXCoord / this.tileSize) * this.tileSize;
        }
        else
        {
            roundedXCoord = Mathf.Floor(roundedXCoord / this.tileSize) * this.tileSize;
        }

        //And we do the same for the local Y position as well
        float roundedYCoord = localPos.y;
        if (roundedYCoord >= 0)
        {
            roundedYCoord = Mathf.Ceil(roundedYCoord / this.tileSize) * this.tileSize;
        }
        else
        {
            roundedYCoord = Mathf.Floor(roundedYCoord / this.tileSize) * this.tileSize;
        }

        /*Sets the local position to the new, rounded grid locations, and
        Zeroing out the local Z pos so everything's on the same plane */
        localPos = new Vector3(roundedXCoord, roundedYCoord, 0);

        //Now we create 2 variables to hold the row and column index locations in our tile array
        int row = 0;
        int col = 0;

        //If this tile is in the Top-Right quadrant
        if (localPos.x >= 0 && localPos.y >= 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x > (this.tilesRight * this.tileSize) || localPos.y > (this.tilesUp * this.tileSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.Abs( Mathf.RoundToInt(localPos.y) - this.tilesUp);
            col = Mathf.RoundToInt(localPos.x) + this.tilesLeft -1;
        }
        //If this tile is in the Top-Left quadrant
        else if (localPos.x < 0 && localPos.y >= 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x < (-this.tilesLeft * this.tileSize) || localPos.y > (this.tilesUp * this.tileSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.Abs( Mathf.RoundToInt(localPos.y) - this.tilesUp);
            col = Mathf.RoundToInt(localPos.x) + this.tilesLeft;
        }
        //If this tile is in the Bottom-Left quadrant
        else if (localPos.x < 0 && localPos.y < 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x < (-this.tilesLeft * this.tileSize) || localPos.y < (-this.tilesDown * this.tileSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.RoundToInt(localPos.y) + this.tilesUp + 1;
            col = Mathf.RoundToInt(localPos.x) + this.tilesLeft;
        }
        //If this tile is in the Bottom-Right quadrant
        else if (localPos.x >= 0 && localPos.y < 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x > (this.tilesRight * this.tileSize) || localPos.y < -(this.tilesDown * this.tileSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.RoundToInt(localPos.y) + this.tilesUp + 1;
            col = Mathf.RoundToInt(localPos.x) + this.tilesLeft - 1;
        }

        //Sets the new tile to that position in the array of tiles
        if (tileToSet_ != null)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Grid Size: " + this.tileGrid.Count + ", " + this.tileGrid[row].Count);
            this.tileGrid[row][col] = tileToSet_;
        }
        //Otherwise, deletes the tile that's currently there
        else
        {
            Debug.Log("DELETE TILE");
            this.tileGrid[row][col] = null;
        }

        //Repaints the this tile map's texture for this location only
        this.PaintSingularTile(row, col, tileToSet_);
    }


    //Function called from Increase/DecreaseGrid. Sets the individual pixels for each tile placed
    private void PaintTexture()
    {
        //Created a new Texture2D that will hold all of the pixel data for this tile map
        Texture2D updatedTexture = new Texture2D(this.pixelWidth * this.tileGrid.Count,
                                                this.pixelWidth * this.tileGrid[0].Count);

        //This variable holds the color of the current pixel
        Color currentPixel;

        //Loops through each row of columns
        for(int r = 0; r < this.tileGrid.Count; ++r)
        {
            //Loops through each column of tiles
            for(int c = 0; c < this.tileGrid[0].Count; ++c)
            {
                //Loops through each pixel for the tile's width
                for(int w = 0; w < this.pixelWidth; ++w)
                {
                    //Loops through each pixel for the tile's height
                    for(int h = 0; h < this.pixelWidth; ++h)
                    {
                        //If the tile is null, we just put a blank, black tile
                        if (this.tileGrid[r][c] == null)
                        {
                            currentPixel = Color.black;
                        }
                        //If the tile isn't null, we find the current pixel on the tile
                        else
                        {
                            //Finding the exact pixel on the source tile map
                            int pixelX = w + (this.tileGrid[r][c].tileTextureCoordsX * this.pixelWidth);
                            int pixelY = h + (this.tileGrid[r][c].tileTextureCoordsY * this.pixelWidth);

                            //currentPixel = this.sourceTileSheet.texture.GetPixel(pixelX, pixelY);
                        }

                        //Sets the pixel color at the correct location on the texture
                        int finalCoordX = (this.pixelWidth * c) + w;
                        int finalCoordY = (this.pixelWidth * r) + h;
                        //updatedTexture.SetPixel(finalCoordX, finalCoordY, currentPixel);
                    }
                }
            }
        }
    }


    //Function called from SetTile. Paints pixels for a single tile instead of the whole texture (like PaintTexture)
    private void PaintSingularTile(int row_ = 0, int col_ = 0, TileInfo newTile_ = null)
    {

    }


    //Function called externally from TileMapEditor. Generates the base XML info for the new file
    public void GenerateBaseXML()
    {
        //If this tile map doesn't have an XML file, we can't do anything
        if (this.xmlFile == null)
        {
             return;
        }

        //Creating a new instance of the serializable TileMap class to write to our XML file
        TileMap createdMap = new TileMap();
        //Setting the attributes of the created Tile Map
        createdMap.TilePixelSize = this.pixelWidth;
        createdMap.TileGridSize = this.tileSize;
        createdMap.TileGrid = this.tileGrid;
        createdMap.TilesUp = this.tilesUp;
        createdMap.TilesDown = this.tilesDown;
        createdMap.TilesLeft = this.tilesLeft;
        createdMap.TilesRight = this.tilesRight;

        //Using an XML serializer and writer, we write this data to our XML file
        XmlSerializer serializer = new XmlSerializer(typeof(TileMap));
        StreamWriter writer = new StreamWriter(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile));
        serializer.Serialize(writer.BaseStream, createdMap);
        writer.Close();


        this.SaveTileMapData();
    }


    //Function called externally from TileMapEditor. Loads in data from a previously existing XML file
    public void LoadExistingXML()
    {

    }


    //Function called internally to write this Tile Map's data to its given XML file
    public void SaveTileMapData()
    {
        Debug.Log(this.name + " U: " + this.tilesUp + ", D: " + this.tilesDown + ", L: " + this.tilesLeft + ", R: " + this.tilesRight);
        //XmlSerializer serializer = new XmlSerializer(typeof(TileMap));
        //StreamWriter writer = new StreamWriter(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile));
        //serializer.Serialize(writer.BaseStream)
    }


    //Create function "GenerateCollider" that creates a custom mesh for this map
    //Create function "CleanUpVerts" that combines duplicate verts on the mesh collider
}