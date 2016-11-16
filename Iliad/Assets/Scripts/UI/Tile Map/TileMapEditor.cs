using UnityEngine;
using UnityEditor;
using System.Xml;

class TileMapEditor : EditorWindow
{
    //The transform that serves as the origin of this tile map
    [HideInInspector]
    public TileMapOrigin mapOrigin;

    //The name of the file that the tile map's data will be called
    private string tileMapFileName = "NewTileMap";

    //The sprite that the current map origin uses as a texture
    public Sprite tileMapSprite;

    //When true, draws the rectangle that makes up this Tile Map's size
    public bool showTileMapSize = false;

    //When true, the user can edit the selected tile map and prevents selecting other objects
    public bool enableMapEditing = false;

    //Boolean that shows or hides the "Dimensions" foldout
    private bool showDimensions = false;

    //Boolean that shows or hides the "Tiles" foldout
    private bool showTiles = false;

    //Vector2 that holds the scroll location for the editor tile scroll view
    private Vector2 scrollPos;

    //Float that determines how zoomed in the tile selection is
    private float zoom = 1.0f;

    //The coordinates on the tile map sprite where the selected tile is
    private int[] selectedTile = new int[] {0, 0};

    //Boolean that shows/hides the editor color foldout
    private bool showColorPicker = false;

    //UI Colors for this editor that the user can change
    private Color outlineColor = Color.green;
    private Color outlineEditColor = Color.red;
    private Color gridColor = Color.red;
    private Color hilightTileColor = Color.green;
    private Color selectedTileColor = Color.blue;




    //Creates a new instance of a Window menu for this Tile Map Editor 
    [MenuItem("Window/Tile Map Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TileMapEditor));
    }


    //Function called when this window is created
    private void OnEnable()
    {
        //Adds a function delegate of our SceneGUI to the delegate of the SceneView GUI so that we can recieve mouse events
        SceneView.onSceneGUIDelegate += this.SceneGUI;

        this.wantsMouseMove = true;
    }


    //Handles the options that appear on this Editor Window that the user can change
    private void OnGUI()
    {
        //If the user is selecting an object that has a Tile Map and a text source, the map's settings can be changed
        if (this.mapOrigin != null && this.mapOrigin.xmlFile != null)
        {
            //Begins a check to see if any properties are changed
            EditorGUI.BeginChangeCheck();

            //Creating a header that displays the name of the tile map object we're editing
            GUILayout.Label("Currently Editing " + this.mapOrigin.name, EditorStyles.largeLabel);

            EditorGUILayout.Space();

            //Setting it so that this whole editor is within a scroll view
            this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, true, true);

            //Creating a fold out section on the window that lets the user set the width and height of the selected tile map
            this.showDimensions = EditorGUILayout.Foldout(this.showDimensions, "Map Details");
            if (this.showDimensions)
            {
                EditorGUI.indentLevel++;

                //Created a field where the user can select the sprite that this map uses as a texture
                this.tileMapSprite = EditorGUILayout.ObjectField("Tile Map Texture Sprite", this.tileMapSprite, typeof(Sprite),
                                                        GUILayout.Width(250)) as Sprite;

                //Created an input field to set the number of pixels shown for each tile
                this.mapOrigin.pixelWidth = EditorGUILayout.IntField("Tile Pixel Size", this.mapOrigin.pixelWidth, GUILayout.Width(200));
                if (this.mapOrigin.pixelWidth < 1)
                {
                    this.mapOrigin.pixelWidth = 1;
                }

                //Created an input field to set the width/height of tiles on the map
                this.mapOrigin.tileSize = EditorGUILayout.FloatField("Tile Grid Height/Width", this.mapOrigin.tileSize, GUILayout.Width(200));


                EditorGUILayout.Space();

                //Grouping the "Tiles Up" and "Tiles Down" input fields on the same line
                EditorGUILayout.BeginHorizontal();
                //Created a placeholderint and input field to set the number of rows above the origin
                int phUP = this.mapOrigin.tilesUp;
                phUP = EditorGUILayout.IntField("Rows Up", phUP, GUILayout.Width(200));

                //Preventing the number of tiles from going below 0
                if (phUP < 0)
                {
                    phUP = 0;
                }

                //If the placeholder int is different from the current value, the current value is changed
                if(phUP != this.mapOrigin.tilesUp)
                {
                    this.mapOrigin.tilesUp = this.mapOrigin.DetermineGridChange(phUP, Directions.Up);
                }

                //Created a placeholder int and input field to set the number of rows below the origin
                int phDown = this.mapOrigin.tilesDown;
                phDown = EditorGUILayout.IntField("Rows Down", phDown, GUILayout.Width(200));

                //Preventing the number of tiles from going below 0
                if (phDown < 0)
                {
                    phDown = 0;
                }

                //If the placeholder int is different from the current value, the current value is changed
                if(phDown != this.mapOrigin.tilesDown)
                {
                    this.mapOrigin.tilesDown = this.mapOrigin.DetermineGridChange(phDown, Directions.Down);
                }
                EditorGUILayout.EndHorizontal();


                //Grouping the "Tiles Left" and "Tiles Down" input fields on the same line
                EditorGUILayout.BeginHorizontal();
                //Created a placeholder int and input field to set the number of columns left of the origin
                int phLeft = this.mapOrigin.tilesLeft;
                phLeft = EditorGUILayout.IntField("Columns Left", phLeft, GUILayout.Width(200));

                //Preventing the number of tiles from going below 0
                if (phLeft < 0)
                {
                    phLeft = 0;
                }

                //If the placeholder int is different from the current value, the current value is changed
                if(phLeft != this.mapOrigin.tilesLeft)
                {
                    this.mapOrigin.tilesLeft = this.mapOrigin.DetermineGridChange(phLeft, Directions.Left);
                }

                //Created a placeholder int and input field to set the number of columns right of the origin
                int phRight = this.mapOrigin.tilesRight;
                phRight = EditorGUILayout.IntField("Columns Right", phRight, GUILayout.Width(200));

                //Preventing the number of tiles from going below 0
                if (phRight < 0)
                {
                    phRight = 0;
                }

                //If the placeholder int is different from the current value, the current value is changed
                if(phRight != this.mapOrigin.tilesRight)
                {
                    this.mapOrigin.tilesRight = this.mapOrigin.DetermineGridChange(phRight, Directions.Right);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            //Creating a fold out section on the window that lets the user set what colors the editor uses
            this.showColorPicker = EditorGUILayout.Foldout(this.showColorPicker, "Editor Colors");
            if(this.showColorPicker)
            {
                EditorGUI.indentLevel++;

                //Color pickers for the rectangle in the scene
                this.outlineColor = EditorGUILayout.ColorField("Map Outline Color", this.outlineColor, GUILayout.MaxWidth(300));
                this.outlineEditColor = EditorGUILayout.ColorField("Map Editing Color", this.outlineEditColor, GUILayout.MaxWidth(300));

                EditorGUILayout.Space();

                //Color pickers for the grid in the editor
                this.gridColor = EditorGUILayout.ColorField("Tile Grid Color", this.gridColor, GUILayout.MaxWidth(300));
                this.hilightTileColor = EditorGUILayout.ColorField("Hilight Tile Color", this.hilightTileColor, GUILayout.MaxWidth(300));
                this.selectedTileColor = EditorGUILayout.ColorField("Selected Tile Color", this.selectedTileColor, GUILayout.MaxWidth(300));

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
            //Created a boolean input box that allows the player to toggle editing mode for the tile map
            this.enableMapEditing = EditorGUILayout.Toggle("Enable Tile Map Editing", this.enableMapEditing);

            EditorGUILayout.Space();

            //Created a boolean input box that allows the player to toggle if the tile map size should be shown
            this.showTileMapSize = EditorGUILayout.Toggle("Show Tile Map Outline", this.showTileMapSize);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            //As long as this tile map has a source texture, this tile section will draw
            if (this.tileMapSprite != null)
            {
                //Created a fold out section on the window that lets the user select what tile they want to place
                this.showTiles = EditorGUILayout.Foldout(this.showTiles, "Tiles");
                if (this.showTiles)
                {
                    EditorGUI.indentLevel++;

                    //Created a slider to let the user determine how zoomed in the tiles are
                    this.zoom = EditorGUILayout.Slider("Zoom", this.zoom, 0.1f, 5, GUILayout.Width(300));

                    EditorGUI.indentLevel--;

                    //Starts a scroll view that displays all tiles on the current source texture
                    using (var scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPos * this.zoom, false, false,
                                                                                GUILayout.MinWidth(Screen.width),
                                                                                GUILayout.MinHeight(Screen.height),
                                                                                GUILayout.Width(this.tileMapSprite.texture.width * this.zoom),
                                                                                GUILayout.Height(this.tileMapSprite.texture.height * this.zoom)))
                    {
                        scrollView.handleScrollWheel = false;

                        //First we draw the full texture for the tile map we're using
                        GUI.DrawTexture(new Rect(0, 0, this.tileMapSprite.texture.width * this.zoom, this.tileMapSprite.texture.height * this.zoom), this.tileMapSprite.texture);

                        //Then we draw lines to slice it into the individual tiles
                        Handles.BeginGUI();
                        Handles.color = this.gridColor;

                        //Loops through a number of times equal to how many tiles wide the texture is
                        for (int w = 0; w < (this.tileMapSprite.texture.width / this.mapOrigin.pixelWidth); ++w)
                        {
                            //Draws vertical lines on the texture
                            Vector3 start = new Vector3(w * this.mapOrigin.pixelWidth * this.zoom, 0);
                            Vector3 end = new Vector3(w * this.mapOrigin.pixelWidth * this.zoom, this.tileMapSprite.texture.height * this.zoom);

                            Handles.DrawLine(start, end);
                        }

                        //Loops through a number of times equal to how many tiles high the texture is
                        for (int h = 0; h < (this.tileMapSprite.texture.height / this.mapOrigin.pixelWidth); ++h)
                        {
                            //Draws horizontal lines on the texture
                            Vector3 start = new Vector3(0, h * this.mapOrigin.pixelWidth * this.zoom);
                            Vector3 end = new Vector3(this.tileMapSprite.texture.width * this.zoom, h * this.mapOrigin.pixelWidth * this.zoom);

                            Handles.DrawLine(start, end);
                        }


                        //Draws a box around the current tile the mouse is on as long as the mouse is over our tiles
                        if (Event.current.mousePosition.x >= 0 && Event.current.mousePosition.y >= 0 &&
                                        Event.current.mousePosition.x < (this.tileMapSprite.texture.width * this.zoom) &&
                                        Event.current.mousePosition.y < (this.tileMapSprite.texture.height * this.zoom))
                        {
                            //Now we draw a box around whichever tile the mouse is over
                            Handles.color = this.hilightTileColor;

                            //Finds the XY coordinate on the tile map texture
                            int xCoord = Mathf.FloorToInt(Event.current.mousePosition.x / (this.mapOrigin.pixelWidth * this.zoom));
                            int yCoord = Mathf.FloorToInt(Event.current.mousePosition.y / (this.mapOrigin.pixelWidth * this.zoom));

                            //Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, Event.current.mousePosition.x, Event.current.mousePosition.y), new Color(0, 1, 0, 0.1f), Color.green);
                            Handles.DrawSolidRectangleWithOutline(new Rect(xCoord * this.mapOrigin.pixelWidth * this.zoom, yCoord * this.mapOrigin.pixelWidth * this.zoom,
                                                                           this.mapOrigin.pixelWidth * this.zoom, this.mapOrigin.pixelWidth * this.zoom),
                                                                           new Color(this.hilightTileColor.r, this.hilightTileColor.g, this.hilightTileColor.b, 0.1f), this.hilightTileColor);

                            //When the player clicks on a new tile, that one is set to the current selection
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            {
                                this.selectedTile[0] = xCoord;
                                this.selectedTile[1] = yCoord;
                            }

                            Repaint();
                        }

                        //Draws a box around the currently selected tile
                        Handles.color = this.selectedTileColor;
                        Handles.DrawSolidRectangleWithOutline(new Rect(this.selectedTile[0] * this.mapOrigin.pixelWidth * this.zoom,
                                                                       this.selectedTile[1] * this.mapOrigin.pixelWidth * this.zoom,
                                                                       this.mapOrigin.pixelWidth * this.zoom,
                                                                       this.mapOrigin.pixelWidth * this.zoom),
                                                                       new Color(this.selectedTileColor.r, this.selectedTileColor.g, this.selectedTileColor.b, 0.1f),
                                                                       this.selectedTileColor);

                        Handles.EndGUI();
                    }
                }
            }

            GUILayout.EndScrollView();

            //If any properties were changed, the scene and selected tile map are set to dirty
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.mapOrigin.gameObject, "Tile Map Editor Change");
                EditorUtility.SetDirty(this.mapOrigin.gameObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }
        //If the user is selecting an object that has a Tile Map but DOESN'T have a text source, they must create one
        else if(this.mapOrigin != null && this.mapOrigin.xmlFile == null)
        {
            //Created a few labels and a text field telling the user that they have to create a file to save map data
            GUILayout.Label("This tile map has no save document.");
            GUILayout.Label("Either set the file in the Tile Map Origin");
            GUILayout.Label("component, or create a new file.");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal(GUILayout.Width(275));
            GUILayout.Label("New tile map file name:  ");
            this.tileMapFileName = GUILayout.TextField(this.tileMapFileName);
            GUILayout.EndHorizontal();

            //Created a button that generates a new text source for the selected tile map
            if(GUILayout.Button("Create New Save"))
            {
                TextAsset xmlFile;

                //First we check to see if a file with that name exists. If not, it's created
                if(Resources.Load("TileMapFiles/" + this.tileMapFileName, typeof(TextAsset)) == null)
                {
                    //Checking to make sure the Resources and TileMapFiles folders exists
                    if(!System.IO.Directory.Exists(Application.dataPath + "/Assets/Resources/"))
                    {
                        //If not, they are created
                        System.IO.DirectoryInfo moo = System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/");
                        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/TileMapFiles/");
                    }
                    //If the Resources folder exists but TileMapFiles doesn't, it's created
                    else if(!System.IO.Directory.Exists(Application.dataPath + "Assets/Resources/TileMapFiles/"))
                    {
                        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/TileMapFiles/");
                    }
                    
                    //If the given file name is allowed (makes it past all of the checks in IsFileNameAllowed function)
                    if(this.IsFileNameAllowed(this.tileMapFileName))
                    {
                        //Created a file path for the new xml file in the Resources folder
                        string filePath = Application.dataPath + "/Resources/TileMapFiles/" + this.tileMapFileName + ".xml";

                        //Creating the file using the file path string and refreshing the asset database so it shows up in editor
                        System.IO.File.WriteAllText(filePath, "");
                        AssetDatabase.Refresh();
                    }
                }


                //Sets the xml source file for the selected map origin to the newly created xml file
                xmlFile = Resources.Load("TileMapFiles/" + this.tileMapFileName, typeof(TextAsset)) as TextAsset;
                this.mapOrigin.xmlFile = xmlFile;

                //Tells the tile map to set up the XML info for the new file
                this.mapOrigin.GenerateBaseXML();

                //Sets the selected tile map and scene as "dirty" which means that they need to be saved
                UnityEditor.Undo.RecordObject(this.mapOrigin.gameObject, "Set XML File as Text Asset");
                EditorUtility.SetDirty(this.mapOrigin.gameObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }

        }
        //Otherwise, there are no options to change because no tile map is selected
        else
        {
            GUILayout.Label("No tile map selected", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Label("Select an object with the TileMapOrigin component", EditorStyles.centeredGreyMiniLabel);
        }
    }


    //Function delegate added to the SceneViewGUI from OnEnable. Lets us use Mouse Events dispatched from that EditorWindow
    private void SceneGUI(SceneView currentScene_)
    {
        //If map editing is enabled
        if (enableMapEditing)
        {
            //Disables object selection so that we don't keep deselecting our tile map
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.type == EventType.mouseDown)
            {
                //Getting the position in world space where the user clicked. The Z coord is zeroed to the tile map's Z plane
                Ray castRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                Vector3 clickPos = new Vector3(castRay.origin.x, castRay.origin.y, this.mapOrigin.transform.position.z);

                //When the user left clicks, we tell the Tile Map Origin that we want to add a tile to the location
                if (Event.current.button == 0)
                {
                    TileInfo addedTile = new TileInfo(this.selectedTile[0], this.selectedTile[1]);          
                    this.mapOrigin.SetTile(clickPos, addedTile);
                }
                //When the user right clicks, we tell the Tile Map Origin that we want to remove a tile at the location
                else if (Event.current.button == 1)
                {
                    this.mapOrigin.SetTile(clickPos);
                }
            }
        }

        //Draws a rectangle on screen to show where the tiles are occupying
        if (this.showTileMapSize && this.mapOrigin != null)
        {
            //If the selected tile map has no XML file, we don't show the rectangle
            if (this.mapOrigin.xmlFile != null)
            {
                this.DrawTileMapRectangle();
            }
        }
    }


    //Function called when the user changes the object(s) they have selected
    private void OnSelectionChange()
    {
        //If the current object selected has a Tile Map component
        if (Selection.gameObjects.GetLength(0) == 1 && Selection.gameObjects[0].GetComponent<TileMapOrigin>() != null)
        {
            //Saves the reference to the Tile Map component
            this.mapOrigin = Selection.gameObjects[0].GetComponent<TileMapOrigin>();
        }
        //If the object selected doesn't have a Tile Map
        else
        {
            //Clears the reference to the Tile Map component
            this.mapOrigin = null;
        }

        //Refreshes this window immediately to either show or hide the user options
        this.Repaint();
    }


    //Function called from SceneGUI. Draws a rectangle outlining the area covered by the tile map's size
    private void DrawTileMapRectangle()
    {
        //Creating an array of points that will make up the rectangle
        Vector3[] verts = new Vector3[]
        {
            //Top Left
            new Vector3(this.mapOrigin.transform.position.x - this.mapOrigin.tilesLeft,
                        this.mapOrigin.transform.position.y + this.mapOrigin.tilesUp, 0) * this.mapOrigin.tileSize,
            //Top Right
            new Vector3(this.mapOrigin.transform.position.x + this.mapOrigin.tilesRight,
                        this.mapOrigin.transform.position.y + this.mapOrigin.tilesUp, 0) * this.mapOrigin.tileSize,
            //Bottom Right
            new Vector3(this.mapOrigin.transform.position.x + this.mapOrigin.tilesRight,
                        this.mapOrigin.transform.position.y - this.mapOrigin.tilesDown, 0) * this.mapOrigin.tileSize,
            //Bottom Left
            new Vector3(this.mapOrigin.transform.position.x - this.mapOrigin.tilesLeft,
                        this.mapOrigin.transform.position.y - this.mapOrigin.tilesDown, 0) * this.mapOrigin.tileSize
        };

        //Creating variables for the box's color
        Color fillColor;
        Color outlineColor;

        //Sets the draw color if the map is being edited
        if (this.enableMapEditing)
        {
            fillColor = new Color(this.outlineEditColor.r, this.outlineEditColor.g, this.outlineEditColor.b, 0.1f);
            outlineColor = this.outlineEditColor;
        }
        //Sets the draw color if the map is NOT being edited
        else
        {
            fillColor = new Color(this.outlineColor.r, this.outlineColor.g, this.outlineColor.b, 0.1f);
            outlineColor = this.outlineColor;
        }

        //Draws the rectangle on screen
        Handles.DrawSolidRectangleWithOutline(verts, fillColor, outlineColor);
    }


    //Function called from OnGUI. Checks the given file name for characters that it shouldn't have
    private bool IsFileNameAllowed(string newFileName_)
    {
        //First we convert the new file name to an array of characters
        char[] newFileChars = newFileName_.ToCharArray();

        //Then we get the list of characters that aren't allowed
        char[] badChars =
        {
            '#',  '%', '&', '{', '}', '/', '<', '>', '*', '~',
            '?', '$', '!', '\'', '\"', '\\', ':', '@', '+', '`',
            '|', '+'
        };

        //If the name of the file is longer than 31 characters, it isn't allowed
        if(newFileChars.Length > 31)
        {
            Debug.LogError("ERROR: The given file name is too long. Please limit the length to 31 chars or fewer.");
            return false;
        }

        //Checks the first character to see if it's a space, period, hyphen, or underline
        if(newFileChars[0] == ' ' || newFileChars[0] == '.' || newFileChars[0] == '_' || newFileChars[0] == '-')
        {
            Debug.LogError("ERROR: The given file name cannot start with a period, space, hyphen, or underline.");
            return false;
        }

        //Checks the last character to see if it's a space, period, hyphen, or underline
        char lastChar = newFileChars[newFileChars.Length - 1];
        if (lastChar == ' ' || lastChar == '.' || lastChar == '_' || lastChar == '-')
        {
            Debug.LogError("ERROR: The given file name cannot end with a period, space, hyphen, or underline.");
            return false;
        }

        //Loop through each character in the new file name
        for (int c = 0; c < newFileChars.Length; c++)
        {
            //We check the current character against all characters that aren't allowed
            for(int b = 0; b < badChars.Length; b++)
            {
                //If a match is found, then the name given is invalid
                if(newFileChars[c] == badChars[b])
                {
                    Debug.LogError("ERROR: The given file name contains at least one invalid character.");
                    return false;
                }
            }
        }

        //If the function got this far, that means the name is good
        return true;
    }
}
