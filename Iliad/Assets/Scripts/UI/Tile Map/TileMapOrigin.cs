using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

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
        this.tileMapInfo.TileGrid = new List<List<TileInfo>>(0);

        //Loops through to add as many rows as we need
        for(int r = 0; r < rows; ++r)
        {
            List<TileInfo> newTileRow = new List<TileInfo>();

            //Loops through to add as many columns as we need
            for(int c = 0; c < cols; ++c)
            {
                newTileRow.Add(new TileInfo());
            }

            //Pushes the new row to our tile grid
            this.tileMapInfo.TileGrid.Add(newTileRow);
        }
    }


    //Function called from TileMapEditor when this tile map's grid is changed. Determines whether IncreaseGrid or DecreaseGrid are called
    public int DetermineGridChange(int newValue_ = 0, Directions changeDirection_ = Directions.None)
    {
        //Making sure that this tile grid is initialized
        if (this.tileMapInfo.TileGrid.Count == null)
        {
            this.InitializeTileGrid();

            /*this.tileMapInfo.TileGrid = new List<List<TileInfo>>();

            //Loops through to add as many columns as we need
            for(int w = 0; w < (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight); ++w)
            {
                List<TileInfo> newListToAdd = new List<TileInfo>();

                //Loops through each column to add as many rows as we need
                for(int h = 0; h < (this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown); ++h)
                {
                    newListToAdd.Add(new TileInfo());
                }

                this.tileMapInfo.TileGrid.Add(newListToAdd);
            }*/
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
        Debug.Log("Increase Grid Start. Grid Size: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[0].Count);
        //Does nothing if the number to add isn't a positive number
        if (numToAdd_ < 1)
            return;
        
        //Inserts new rows at the beginning of the first list
        if (direction_ == Directions.Up)
        {
            this.tileMapInfo.TilesUp += numToAdd_;

            //Loops through a number of times equal to the num to add
            for (int n = 0; n < numToAdd_; ++n)
            {
                List<TileInfo> newTopRow = new List<TileInfo>(this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight) { new TileInfo() };
                this.tileMapInfo.TileGrid.Insert(0, newTopRow);
            }
        }
        //Adds new rows at the end of the first list
        else if (direction_ == Directions.Down)
        {
            this.tileMapInfo.TilesDown += numToAdd_;

            //Loops through a number of times equal to the num to add
            for (int n = 0; n < numToAdd_; ++n)
            {
                List<TileInfo> newBottomRow = new List<TileInfo>(this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight) { new TileInfo() };
                this.tileMapInfo.TileGrid.Add(newBottomRow);
            }
        }
        //Loops through each row in the first list and inserts new columns at the beginning of the inner lists
        else if (direction_ == Directions.Left)
        {
            this.tileMapInfo.TilesLeft += numToAdd_;

            //Loops through each row
            for (int r = 0; r < this.tileMapInfo.TileGrid.Count; ++r)
            {
                for (int n = 0; n < numToAdd_; ++n)
                {
                    this.tileMapInfo.TileGrid[r].Insert(0, new TileInfo());
                }
            }
        }
        //Loops through each row in the first list and inserts new columns at the end of the inner lists
        else if (direction_ == Directions.Right)
        {
            this.tileMapInfo.TilesRight += numToAdd_;

            //Loops through each row
            for (int r = 0; r < this.tileMapInfo.TileGrid.Count; ++r)
            {
                for (int n = 0; n < numToAdd_; ++n)
                {
                    this.tileMapInfo.TileGrid[r].Add(new TileInfo());
                }
            }
        }

        Debug.Log("Call Paint Texture function still commented out");
        //Repaints the this tile map's texture
        //this.PaintTexture();
        Debug.Log("Increase Grid End. Grid Size: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[0].Count);
    }


    //Function that decreases the tile grid in the direction given
    public void DecreaseGrid(Directions direction_ = Directions.Right, int numToRemove_ = 1)
    {
        Debug.Log("Decrease Grid Start. Grid Size: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[0].Count);
        //Does nothing if the number to remove isn't a positive number
        if (numToRemove_ < 1)
            return;
        
        int tilesRemoved = numToRemove_;

        //Removes rows at the end of the first list
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
            Debug.Log("Up: " + this.tileMapInfo.TilesUp);

            //Loops through a number of times equal to the rows removed
            for (int n = 0; n < tilesRemoved; ++n)
            {
                Debug.Log("Inside first loop");
                //Nulls and destroys each tile in the removed row
                for (int r = 0; r < this.tileMapInfo.TileGrid[0].Count; ++r)
                {
                    Debug.Log("Inside second loop");
                    this.tileMapInfo.TileGrid[0][r] = null;
                }

                Debug.Log("Before remove");
                Debug.Log("Before remove UP: " + this.tileMapInfo.TileGrid.Count);
                this.tileMapInfo.TileGrid.RemoveAt(0);
                Debug.Log("After remove UP: " + this.tileMapInfo.TileGrid.Count);
            }
        }
        //Removes rows at the end of the first list
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
            Debug.Log("Down: " + this.tileMapInfo.TilesDown);
            
            //Loops through a number of times equal to the rows removed
            for (int n = 0; n < tilesRemoved; ++n)
            {
                //Nulls and destroys each tile in the removed row
                for (int r = 0; r < this.tileMapInfo.TileGrid[0].Count; ++r)
                {
                    this.tileMapInfo.TileGrid[this.tileMapInfo.TileGrid.Count - 1][r] = null;
                }

                this.tileMapInfo.TileGrid.RemoveAt(this.tileMapInfo.TileGrid.Count - 1);
            }
        }
        //Loops through each row in the first list and removes columns at the beginning of the inner lists
        else if (direction_ == Directions.Left)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tileMapInfo.TilesLeft)
            {
                tilesRemoved = this.tileMapInfo.TilesLeft;
            }

            this.tileMapInfo.TilesLeft -= tilesRemoved;

            //Loops through each row
            for (int r = 0; r < this.tileMapInfo.TileGrid.Count; ++r)
            {
                //Destroys, nulls, and removes the first tile in each row
                for (int n = 0; n < tilesRemoved; ++n)
                {
                    this.tileMapInfo.TileGrid[r][0] = null;
                    this.tileMapInfo.TileGrid[r].RemoveAt(0);
                }
            }
        }
        else if (direction_ == Directions.Right)
        {
            //Makes sure that we can't subtract from a direction enough to drop below 0
            if (tilesRemoved > this.tileMapInfo.TilesRight)
            {
                tilesRemoved = this.tileMapInfo.TilesRight;
            }
            
            this.tileMapInfo.TilesRight -= tilesRemoved;
            
            //Loops through each row
            for (int r = 0; r < this.tileMapInfo.TileGrid.Count; ++r)
            {
                //Destroys, nulls, and removes the last tile in each row
                for (int n = 0; n < tilesRemoved; ++n)
                {
                    this.tileMapInfo.TileGrid[r][this.tileMapInfo.TileGrid[r].Count - 1] = null;
                    this.tileMapInfo.TileGrid[r].RemoveAt(this.tileMapInfo.TileGrid[r].Count - 1);
                }
            }
        }

        //Repaints the this tile map's texture
        this.PaintTexture();

        Debug.Log("Decrease Grid End. Grid Size: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[0].Count);
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
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Grid Size: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[row].Count);
            this.tileMapInfo.TileGrid[row][col] = tileToSet_;
        }
        //Otherwise, deletes the tile that's currently there
        else
        {
            Debug.Log("DELETE TILE");
            this.tileMapInfo.TileGrid[row][col] = null;
        }

        //Repaints the this tile map's texture for this location only
        this.PaintSingularTile(row, col, tileToSet_);
    }


    //Function called from Increase/DecreaseGrid. Sets the individual pixels for each tile placed
    private void PaintTexture()
    {
        //Created a new Texture2D that will hold all of the pixel data for this tile map
        Texture2D updatedTexture = new Texture2D(this.tileMapInfo.TilePixelSize * this.tileMapInfo.TileGrid.Count,
                                                this.tileMapInfo.TilePixelSize * this.tileMapInfo.TileGrid[0].Count);

        //This variable holds the color of the current pixel
        Color currentPixel;

        //Loops through each row of columns
        for(int r = 0; r < this.tileMapInfo.TileGrid.Count; ++r)
        {
            //Loops through each column of tiles
            for(int c = 0; c < this.tileMapInfo.TileGrid[0].Count; ++c)
            {
                //Loops through each pixel for the tile's width
                for(int w = 0; w < this.tileMapInfo.TilePixelSize; ++w)
                {
                    //Loops through each pixel for the tile's height
                    for(int h = 0; h < this.tileMapInfo.TilePixelSize; ++h)
                    {
                        //If the tile is null, we just put a blank, black tile
                        if (this.tileMapInfo.TileGrid[r][c] == null)
                        {
                            currentPixel = Color.black;
                        }
                        //If the tile isn't null, we find the current pixel on the tile
                        else
                        {
                            //Finding the exact pixel on the source tile map
                            int pixelX = w + (this.tileMapInfo.TileGrid[r][c].tileTextureCoordsX * this.tileMapInfo.TilePixelSize);
                            int pixelY = h + (this.tileMapInfo.TileGrid[r][c].tileTextureCoordsY * this.tileMapInfo.TilePixelSize);

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


    //Function called externally from TileMapEditor. Generates the base XML info for the new file
    public void GenerateBaseXML()
    {
        //If this tile map doesn't have an XML file, we can't do anything
        if (this.xmlFile == null)
        {
             return;
        }

        //Creating a new, default TileMap class to be serialized and saved
        TileMap newTileMap = new TileMap();

        //Using an XML serializer and writer, we write this data to our XML file
        XmlSerializer serializer = new XmlSerializer(typeof(TileMap));
        StreamWriter writer = new StreamWriter(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile));
        serializer.Serialize(writer.BaseStream, newTileMap);
        writer.Close();


        //Saving the new, default TileMap as our own
        this.tileMapInfo = newTileMap;
    }


    //Function called externally from TileMapEditor. Loads in data from a previously existing XML file
    public void LoadExistingXML()
    {
        //Can't load an existing XML if it doesn't exist....
        if (this.xmlFile == null)
            return;

        Debug.Log("LoadExistingXML file name: " + this.xmlFile.name);

        //Creating a new XML document instance to load the data
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(this.xmlFile.text);

        //Making sure that the tileMapInfo isn't null before using it
        if(this.tileMapInfo == null)
        {
            this.tileMapInfo = new TileMap();
        }
        //Converting the text from the xml file into a byte array
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(this.xmlFile.text);

        //Then we create a memory stream for the byte array
        MemoryStream memStream = new MemoryStream(byteArray);
        XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.UTF8);

        //Deserialize the memory stream to load in the TileMap data
        XmlSerializer serializer = new XmlSerializer(typeof(TileMap));
        
        /*System.Xml.XmlReader xReader = System.Xml.XmlReader.
        System.Xml.Linq.XDocument testLoadedMap = System.Xml.Linq.XDocument.Load(new XmlReader)*/
        System.Xml.Linq.XElement xElem = System.Xml.Linq.XElement.Load(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile));
        IEnumerable<System.Xml.Linq.XElement> xNode = xElem.Elements();

        TileMap testMap = new TileMap();
        testMap = serializer.Deserialize(xElem.CreateReader()) as TileMap;
        
        List<List<TileInfo>> testGrid = new List<List<TileInfo>>();

        //Debug.Log("TestMap height: " + (testMap.TilesUp + testMap.TilesDown) + ", width: " + (testMap.TilesLeft + testMap.TilesRight));
        //Debug.Log("TestMap Grid: " + testMap.TileGrid.Count + ", " + testMap.TileGrid[0].Count);

        //Loops through and copy each row and height of tiles that are SUPPOSED to be there
        for (int height=0; height < (testMap.TilesUp + testMap.TilesDown); ++height)
        {
            testGrid.Add(new List<TileInfo>(testMap.TilesLeft + testMap.TilesRight));

            for(int width=0; width < (testMap.TilesLeft + testMap.TilesRight); ++width)
            {
                Debug.Log("Height: " + height + ", Width: " + width + " (Size: " + testMap.TileGrid.Count + "," + testMap.TileGrid[0].Count + ")");

                TileInfo currentTile = testMap.TileGrid[height][width];
                Debug.Log("MOO");
                testGrid[height].Add(currentTile);
            }

            Debug.Log("New row----------------------------------------------");
        }

        Debug.Log("Test grid size: " + testGrid.Count + ", " + testGrid[0].Count);
        testMap.TileGrid = testGrid;
        this.tileMapInfo = testMap;
        return;

        /*List<List<TileInfo>> testGrid = serializer.Deserialize(xElem.Element("TileGrid").CreateReader()) as List<List<TileInfo>>;
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@ grid size: " + testGrid.Count + ", " + testGrid[0].Count);
        Debug.Log("LoadExistingXML, testMap grid size: " + testMap.TileGrid.Count + ", " + testMap.TileGrid[0].Count);
        foreach(var node in xNode)
        {
            Debug.Log(node.Value);
        }*/


        TileMap loadedMap = serializer.Deserialize(memStream) as TileMap;
        //this.tileMapInfo = serializer.Deserialize(memStream) as TileMap;

        Debug.Log("LoadExistingXML, existing map grid size start: " + loadedMap.TileGrid.Count + ", " + loadedMap.TileGrid[0].Count);

        for(int r = 0; r < loadedMap.TileGrid.Count; ++r)
        {
            for(int p = 0; p < loadedMap.TileGrid[r].Count; ++p)
            {
                Debug.Log("Pos " + r + ", " + p + ": " + loadedMap.TileGrid[r][p].tileTestColor);
            }
        }

        Debug.Log("LoadExistingXML, existing map grid size end: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[0].Count);
        this.tileMapInfo = loadedMap;
    }


    //Function called internally to write this Tile Map's data to its given XML file
    public void SaveTileMapData()
    {
        Debug.Log("SaveTileMapData Height/Width: " + (this.tileMapInfo.TilesUp + this.tileMapInfo.TilesDown) + ", " + (this.tileMapInfo.TilesLeft + this.tileMapInfo.TilesRight));
        Debug.Log("SaveTileMapData Grid Size: " + this.tileMapInfo.TileGrid.Count + ", " + this.tileMapInfo.TileGrid[0].Count);

        //Using an XML serializer and writer, we write this data to our XML file
        XmlSerializer serializer = new XmlSerializer(typeof(TileMap));
        StreamWriter writer = new StreamWriter(UnityEditor.AssetDatabase.GetAssetPath(this.xmlFile));
        serializer.Serialize(writer.BaseStream, this.tileMapInfo);
        writer.Close();

        //Refreshes the asset database so that we can look at the file and make sure it saved
        UnityEditor.AssetDatabase.Refresh();
    }


    //Create function "GenerateCollider" that creates a custom mesh for this map
    //Create function "CleanUpVerts" that combines duplicate verts on the mesh collider
}