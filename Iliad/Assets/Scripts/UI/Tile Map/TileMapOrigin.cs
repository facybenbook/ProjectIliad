using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class TileMapOrigin : MonoBehaviour
{
    //The TileMap class that we save to the XML file
    public TileMap tileMapInfo = new TileMap();

    //The file path that holds this tile map's XML data file
    public TextAsset xmlFile;

    //The source image that this tile map uses to texture each tile
    [HideInInspector]
    public Texture sourceTileSheet = null;


    
    
    //Function called from DetermineGridChange. Makes sure our tile map grid is initialized properly
    private void InitializeTileGrid()
    {
        //Getting the rows and columns for the grid
        int rows = this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown;
        int cols = this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight;

        //Initializing the 2 dimensional list
        this.tileMapInfo.TileGrid = new List<TileInfo>(rows * cols);

        //Loops through to add as many rows as we need
        for(int t = 0; t < this.tileMapInfo.TileGrid.Count; ++t)
        {
            this.tileMapInfo.TileGrid[t] = new TileInfo();
        }
    }


    //Function called from TileMapEditor when this tile map's grid is changed. Determines whether IncreaseGrid or DecreaseGrid are called
    public int DetermineGridChange(int newValue_ = 0, Directions changeDirection_ = Directions.None)
    {
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
                current = this.tileMapInfo.TilesUp;
                break;

            case Directions.Down:
                current = this.tileMapInfo.TilesDown;
                break;

            case Directions.Left:
                current = this.tileMapInfo.TilesLeft;
                break;

            case Directions.Right:
                current = this.tileMapInfo.TilesRight;
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
        Debug.Log("Increase Grid Start. Grid Size: " + this.tileMapInfo.TileGrid.Count);
        //Does nothing if the number to add isn't a positive number
        if (numToAdd_ < 1)
            return;
        
        //Inserts new rows at the beginning of the first list
        if (direction_ == Directions.Up)
        {
            this.tileMapInfo.TilesUp += numToAdd_;

            int totalTilesToAdd = numToAdd_ * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight);

            //Loops through a number of times equal to the num to add
            for (int n = 0; n < totalTilesToAdd; ++n)
            {
                //Inserts a new tile at the beginning of the current list
                this.tileMapInfo.TileGrid.Insert(0, new TileInfo());
            }
        }
        //Adds new rows at the end of the first list
        else if (direction_ == Directions.Down)
        {
            this.tileMapInfo.TilesDown += numToAdd_;

            int totalTilesToAdd = numToAdd_ * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight);

            //Loops through a number of times equal to the num to add
            for (int n = 0; n < totalTilesToAdd; ++n)
            {
                //Inserts a new tile at the end of the current list
                int posToAdd = this.tileMapInfo.TileGrid.Count - 1;
                this.tileMapInfo.TileGrid.Insert(posToAdd, new TileInfo());
            }
        }
        //Loops through each row in the first list and inserts new columns at the beginning of the inner lists
        else if (direction_ == Directions.Left)
        {
            this.tileMapInfo.TilesLeft += numToAdd_;

            //Getting the number of rows and columns that make up this grid
            int row = this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown;
            int col = this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight;

            //Loops through each row
            for (int r = 0; r < row; ++r)
            {
                //Loops through each column
                for (int c = 0; c < col; ++c)
                {
                    //If we're at the beginning of a new row, we need to add empty tiles
                    if (c < numToAdd_)
                    {
                        //Finding the exact spot in the list using the row and column offsets
                        int tilePos = (r * row) + c;
                        //Inserting a new tile at the position
                        this.tileMapInfo.TileGrid.Insert(tilePos, new TileInfo());
                        //Increasing the column counter since we added to the count
                        c += 1;
                    }
                }
            }
        }
        //Loops through each row in the first list and inserts new columns at the end of the inner lists
        else if (direction_ == Directions.Right)
        {
            this.tileMapInfo.TilesRight += numToAdd_;

            //Getting the number of rows and columns that make up this grid
            int row = this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown;
            int col = this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight;

            //Loops through each row
            for (int r = 0; r < row; ++r)
            {
                //Loops through each column
                for (int c = 0; c < col; ++c)
                {
                    //If we're at the end of the row, we need to add empty tiles
                    if (c > (col - numToAdd_))
                    {
                        //Finding the exact spot in the list using the row and column offsets
                        int tilePos = (r * row) + c;
                        //Inserting a new tile at the position
                        this.tileMapInfo.TileGrid.Insert(tilePos, new TileInfo());
                        //Increasing the column counter since we added to the count
                        c += 1;
                    }
                }
            }
        }

        Debug.Log("Call Paint Texture function still commented out");
        //Repaints the this tile map's texture
        //this.PaintTexture();
        Debug.Log("Increase Grid End. Grid Size: " + this.tileMapInfo.TileGrid.Count);
    }


    //Function that decreases the tile grid in the direction given
    public void DecreaseGrid(Directions direction_ = Directions.Right, int numToRemove_ = 1)
    {
        Debug.Log("Decrease Grid Start. Grid Size: " + this.tileMapInfo.TileGrid.Count);
        //Does nothing if the number to remove isn't a positive number
        if (numToRemove_ < 1)
            return;
        
        int tilesRemoved = numToRemove_;

        //Removes rows starting from the beginning of the list
        if (direction_ == Directions.Up)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tileMapInfo.TilesUp)
            {
                tilesRemoved = this.tileMapInfo.TilesUp;
            }

            //Makes sure the grid is at least 1 tile high
            if (this.tileMapInfo.TilesDown == 0 && (this.tileMapInfo.TilesUp - tilesRemoved) == 0)
            {
                tilesRemoved -= 1;
                Debug.LogWarning("WARNING: The tile map grid needs to be at least 1 tile high");
            }

            this.tileMapInfo.TilesUp -= tilesRemoved;

            //Find the number of individual tiles to remove based on how many tiles are in a row
            int tilesToPop = tilesRemoved * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight);

            //Removes the number of needed tiles starting from the beginning
            this.tileMapInfo.TileGrid.RemoveRange(0, tilesToPop);
        }
        //Removes rows at the end of the list
        else if (direction_ == Directions.Down)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tileMapInfo.TilesDown)
            {
                tilesRemoved = this.tileMapInfo.TilesDown;
            }

            //Makes sure the grid is at least 1 tile high
            if (this.tileMapInfo.TilesUp == 0 && (this.tileMapInfo.TilesDown - tilesRemoved) == 0)
            {
                tilesRemoved -= 1;
                Debug.LogWarning("WARNING: The tile map grid needs to be at least 1 tile high");
            }

            this.tileMapInfo.TilesDown -= tilesRemoved;

            //Find the number of individual tiles to remove based on how many tiles are in a row
            int tilesToPop = tilesRemoved * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight);

            //Finds the index where we should start removing tiles
            int endPoint = this.tileMapInfo.TileGrid.Count - tilesToPop;

            //Removes the number of needed tiles starting from the end point
            this.tileMapInfo.TileGrid.RemoveRange(endPoint, tilesToPop);
        }
        //Removes tiles at the beginning of each row
        else if (direction_ == Directions.Left)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tileMapInfo.TilesLeft)
            {
                tilesRemoved = this.tileMapInfo.TilesLeft;
            }

            //Makes sure the grid is at least 1 tile wide
            if (this.tileMapInfo.TilesLeft == 0 && (this.tileMapInfo.TilesRight - tilesRemoved) == 0)
            {
                tilesRemoved -= 1;
                Debug.LogWarning("WARNING: The tile map grid needs to be at least 1 tile wide");
            }

            this.tileMapInfo.TilesLeft -= tilesRemoved;

            //Getting the number of rows and columns that make up this grid
            int row = this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown;
            int col = this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight;

            //Loops through each row
            for (int r = 0; r < row; ++r)
            {
                //The index where we start removing tiles from in this loop
                int startIndex = r * col;

                //Removing the correct number of tiles from the start of this row
                this.tileMapInfo.TileGrid.RemoveRange(startIndex, tilesRemoved);
            }
        }
        //Removes tiles at the end of each row
        else if (direction_ == Directions.Right)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tileMapInfo.TilesRight)
            {
                tilesRemoved = this.tileMapInfo.TilesRight;
            }

            //Makes sure the grid is at least 1 tile wide
            if (this.tileMapInfo.TilesRight == 0 && (this.tileMapInfo.TilesLeft - tilesRemoved) == 0)
            {
                tilesRemoved -= 1;
                Debug.LogWarning("WARNING: The tile map grid needs to be at least 1 tile wide");
            }

            this.tileMapInfo.TilesRight -= tilesRemoved;

            //Getting the number of rows and columns that make up this grid
            int row = this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown;
            int col = this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight;

            //Loops through each row
            for (int r = 0; r < row; ++r)
            {
                //The index where we start removing tiles from in this loop
                int startIndex = (r * col) + (row - tilesRemoved);

                //Removing the correct number of tiles from the end of this row
                this.tileMapInfo.TileGrid.RemoveRange(startIndex, tilesRemoved);
            }
        }

        //Repaints the this tile map's texture
        this.PaintTexture();

        Debug.Log("Decrease Grid End. Grid Size: " + this.tileMapInfo.TileGrid.Count);
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
            roundedXCoord = Mathf.Ceil(roundedXCoord / this.tileMapInfo.TileGridSize) * this.tileMapInfo.TileGridSize;
        }
        else
        {
            roundedXCoord = Mathf.Floor(roundedXCoord / this.tileMapInfo.TileGridSize) * this.tileMapInfo.TileGridSize;
        }

        //And we do the same for the local Y position as well
        float roundedYCoord = localPos.y;
        if (roundedYCoord >= 0)
        {
            roundedYCoord = Mathf.Ceil(roundedYCoord / this.tileMapInfo.TileGridSize) * this.tileMapInfo.TileGridSize;
        }
        else
        {
            roundedYCoord = Mathf.Floor(roundedYCoord / this.tileMapInfo.TileGridSize) * this.tileMapInfo.TileGridSize;
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
            if (localPos.x > (this.tileMapInfo.TilesRight * this.tileMapInfo.TileGridSize) || localPos.y > (this.tileMapInfo.TilesUp * this.tileMapInfo.TileGridSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.Abs( Mathf.RoundToInt(localPos.y) - this.tileMapInfo.TilesUp);
            col = Mathf.RoundToInt(localPos.x) + this.tileMapInfo.TilesLeft -1;
        }
        //If this tile is in the Top-Left quadrant
        else if (localPos.x < 0 && localPos.y >= 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x < (-this.tileMapInfo.TilesLeft * this.tileMapInfo.TileGridSize) || localPos.y > (this.tileMapInfo.TilesUp * this.tileMapInfo.TileGridSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.Abs( Mathf.RoundToInt(localPos.y) - this.tileMapInfo.TilesUp);
            col = Mathf.RoundToInt(localPos.x) + this.tileMapInfo.TilesLeft;
        }
        //If this tile is in the Bottom-Left quadrant
        else if (localPos.x < 0 && localPos.y < 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x < (-this.tileMapInfo.TilesLeft * this.tileMapInfo.TileGridSize) || localPos.y < (-this.tileMapInfo.TilesDown * this.tileMapInfo.TileGridSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.RoundToInt(localPos.y) + this.tileMapInfo.TilesUp + 1;
            col = Mathf.RoundToInt(localPos.x) + this.tileMapInfo.TilesLeft;
        }
        //If this tile is in the Bottom-Right quadrant
        else if (localPos.x >= 0 && localPos.y < 0)
        {
            //Does nothing if the X or Y coords are out of bounds
            if (localPos.x > (this.tileMapInfo.TilesRight * this.tileMapInfo.TileGridSize) || localPos.y < -(this.tileMapInfo.TilesDown * this.tileMapInfo.TileGridSize))
            {
                return;
            }

            //Saves the location of this tile in the 2D tile grid array
            row = Mathf.RoundToInt(localPos.y) + this.tileMapInfo.TilesUp + 1;
            col = Mathf.RoundToInt(localPos.x) + this.tileMapInfo.TilesLeft - 1;
        }

        //Sets the new tile to that position in the array of tiles
        if (tileToSet_ != null)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Grid Size: " + this.tileMapInfo.TileGrid.Count);

            //Finding the correct index in the grid to set
            int indexToChange = (row * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight)) + col;
            this.tileMapInfo.TileGrid[indexToChange] = tileToSet_;
        }
        //Otherwise, replaces the tile that's currently there with an empty tile
        else
        {
            Debug.Log("DELETE TILE");

            //Finding the correct index in the grid to set
            int indexToChange = (row * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight)) + col;
            this.tileMapInfo.TileGrid[indexToChange] = new TileInfo();
        }

        //Repaints the this tile map's texture for this location only
        this.PaintSingularTile(row, col, tileToSet_);
    }


    //Function called from Increase/DecreaseGrid. Sets the individual pixels for each tile placed
    private void PaintTexture()
    {
        //Getting the number of rows and columns for reference
        int row = this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown;
        int col = this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight;

        //Created a new Texture2D that will hold all of the pixel data for this tile map
        Texture2D updatedTexture = new Texture2D(this.tileMapInfo.TilePixelSize * row,
                                                this.tileMapInfo.TilePixelSize * col);

        //This variable holds the color of the current pixel
        Color currentPixel;

        //Loops through each row of columns
        for(int r = 0; r < row; ++r)
        {
            //Loops through each column of tiles
            for(int c = 0; c < col; ++c)
            {
                //Loops through each pixel for the tile's width
                for(int w = 0; w < this.tileMapInfo.TilePixelSize; ++w)
                {
                    //Loops through each pixel for the tile's height
                    for(int h = 0; h < this.tileMapInfo.TilePixelSize; ++h)
                    {
                        //If the tile is null, we just put a blank, black tile
                        if (this.tileMapInfo.TileGrid[GetRowColIndex(r, c)] == null)
                        {
                            currentPixel = Color.black;
                        }
                        //If the tile isn't null, we find the current pixel on the tile
                        else
                        {
                            //Finding the exact pixel on the source tile map
                            int pixelX = w + (this.tileMapInfo.TileGrid[GetRowColIndex(r, c)].tileTextureCoordsX * this.tileMapInfo.TilePixelSize);
                            int pixelY = h + (this.tileMapInfo.TileGrid[GetRowColIndex(r, c)].tileTextureCoordsY * this.tileMapInfo.TilePixelSize);

                            //currentPixel = this.sourceTileSheet.texture.GetPixel(pixelX, pixelY);
                        }

                        //Sets the pixel color at the correct location on the texture
                        int finalCoordX = (this.tileMapInfo.TilePixelSize * c) + w;
                        int finalCoordY = (this.tileMapInfo.TilePixelSize * r) + h;
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


    //Function called externally from TileMapEditor. Loads in data from a previously existing XML file
    public void LoadExistingXML()
    {
        //Can't load an existing XML if it doesn't exist....
        if (this.xmlFile == null)
            return;

        //Loading the TileMap class from our text file
        TileMap loadedMap = JsonUtility.FromJson<TileMap>(this.xmlFile.text);

        Debug.Log("Loaded JSON map grid size: " + loadedMap.TileGrid.Count);

        //Setting the loaded map as the one we'll use from here on out
        this.tileMapInfo = loadedMap;
    }


    //Function called internally to write this Tile Map's data to its given XML file
    public void SaveTileMapData()
    {
        //Makes sure that this tile map origin actually has a tile map class
        if(this.tileMapInfo == null)
        {
            this.tileMapInfo = new TileMap();
        }

        Pie myPie = new Pie();
        string jsonPie = UnityEditor.EditorJsonUtility.ToJson(myPie, true);
        Debug.Log("My JSON Pie: " + jsonPie);

        //Clearing the string from our text file if needed
        if(this.xmlFile.text != "")
        {
            FileStream fstream = new FileStream(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile), FileMode.Truncate);
        }

        Debug.Log("File text (should be empty): " + this.xmlFile.text);

        //Creating the string that holds our JSON information
        string jsonString = UnityEditor.EditorJsonUtility.ToJson(this.tileMapInfo, true);

        Debug.Log("JSON string: " + jsonString);

        //Saving our current TileMap class as a JSON string to our text file
        StreamWriter writer = new StreamWriter(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile));
        writer.WriteLine(jsonString);
        writer.Close();

        //Refreshes the asset database so that we can look at the file and make sure it saved
        UnityEditor.AssetDatabase.Refresh();
    }


    //Private function used to find the correct index in the list when given a row and column
    private int GetRowColIndex(int row_, int col_)
    {
        //Making sure that both the row and column given aren't less than 0
        if(row_ < 0 || col_ < 0)
        {
            Debug.LogError("ERROR! TileMapOrigin.GetRowColIndex, parameters CANNOT be less than 0!");
            return 0;
        }

        //Creating an int that will be returned
        int returnIndex = 0;

        //Getting the index using the row and column given
        returnIndex += row_ * (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight);
        returnIndex += col_;

        return returnIndex;
    }

    //Create function "GenerateCollider" that creates a custom mesh for this map
    //Create function "CleanUpVerts" that combines duplicate verts on the mesh collider
}


[Serializable]
public class Pie : System.Object
{
    [SerializeField]
    public List<TileInfo> pieTiles;

    [SerializeField]
    public float notTitanic;

    [SerializeField]
    public TileMap questionMark;


    public Pie()
    {
        pieTiles = new List<TileInfo>()
        {
            new TileInfo(TestColors.Green)
        };

        notTitanic = 0.5f;

        questionMark = new TileMap();
    }
}