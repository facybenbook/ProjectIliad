using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementLogic : MonoBehaviour
{
    //References to the keys that move the player
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode upKeyAlt = KeyCode.W;

    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode downKeyAlt = KeyCode.S;

    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode leftKeyAlt = KeyCode.A;

    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode rightKeyAlt = KeyCode.D;

    //The distance from one tile to the next that this player moves
    public float distToMove = 1.0f;
    private float currentDistTraveled = 0;

    //The time it takes for this player to move one tile
    public float timeToMove = 0.15f;
    private float moveTimeCounter = 0;

    //The movement "velocity" (not using rigid bodies, just transforms) based on time and distance
    private float moveVelocity = 0;

    //The current direction the player is facing
    public Directions directionFacing = Directions.Down;

    //The delay when changing direction until the player starts moving that direction
    public float changeDirectionDelay = 0.1f;
    private float changeDirectionCounter = 0;

    //Bools that track if the player was moving during the last 3 frames
    private List<bool> wasMovingLastFrame = new List<bool>(3) { false, false, false };



    //Private function called when this object loads in
    private void Awake()
    {
        this.moveVelocity = this.distToMove / this.timeToMove;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        //Counts down the change direction counter
        if (this.changeDirectionCounter > 0)
        {
            this.changeDirectionCounter -= Time.fixedDeltaTime;
        }

        //Counts down the movement counter and moves the player as long as the move timer has time remaining
        if (this.moveTimeCounter > 0)
        {
            this.moveTimeCounter -= Time.fixedDeltaTime;

            //Tracks how far we've moved so far, so that we can bump it up or down in case the velocity or delta time is off
            this.currentDistTraveled += this.moveVelocity * Time.fixedDeltaTime;

            switch (this.directionFacing)
            {
                case Directions.Up:
                    this.transform.position += new Vector3(0, this.moveVelocity, 0) * Time.fixedDeltaTime;

                    //Corrects any offsets that make the player move too far
                    if (this.currentDistTraveled > this.distToMove)
                    {
                        //Stops the time counter to prevent us from moving any more
                        this.moveTimeCounter = 0;
                        //Moves the player back so that they only traveled the distance that they should have
                        this.transform.position -= new Vector3(0, (this.currentDistTraveled - this.distToMove), 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    //Corrects any offsets that made the player not move far enough
                    else if (this.currentDistTraveled < this.distToMove && this.moveTimeCounter <= 0)
                    {
                        //Moves the player forward so they finish traveling the remaining distance
                        this.transform.position += new Vector3(0, (this.distToMove - this.currentDistTraveled), 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    break;

                case Directions.Down:
                    this.transform.position += new Vector3(0, -this.moveVelocity, 0) * Time.fixedDeltaTime;

                    //Corrects any offsets that make the player move too far
                    if (this.currentDistTraveled > this.distToMove)
                    {
                        //Stops the time counter to prevent us from moving any more
                        this.moveTimeCounter = 0;
                        //Moves the player back so that they only traveled the distance that they should have
                        this.transform.position -= new Vector3(0, -(this.currentDistTraveled - this.distToMove), 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    //Corrects any offsets that made the player not move far enough
                    else if (this.currentDistTraveled < this.distToMove && this.moveTimeCounter <= 0)
                    {
                        //Moves the player forward so they finish traveling the remaining distance
                        this.transform.position += new Vector3(0, -(this.distToMove - this.currentDistTraveled), 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    break;

                case Directions.Left:
                    this.transform.position += new Vector3(-this.moveVelocity, 0, 0) * Time.fixedDeltaTime;

                    //Corrects any offsets that make the player move too far
                    if (this.currentDistTraveled > this.distToMove)
                    {
                        //Stops the time counter to prevent us from moving any more
                        this.moveTimeCounter = 0;
                        //Moves the player back so that they only traveled the distance that they should have
                        this.transform.position -= new Vector3(-(this.currentDistTraveled - this.distToMove), 0, 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    //Corrects any offsets that made the player not move far enough
                    else if (this.currentDistTraveled < this.distToMove && this.moveTimeCounter <= 0)
                    {
                        //Moves the player forward so they finish traveling the remaining distance
                        this.transform.position += new Vector3(-(this.distToMove - this.currentDistTraveled), 0, 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    break;

                case Directions.Right:
                    this.transform.position += new Vector3(this.moveVelocity, 0, 0) * Time.fixedDeltaTime;

                    //Corrects any offsets that make the player move too far
                    if (this.currentDistTraveled > this.distToMove)
                    {
                        //Stops the time counter to prevent us from moving any more
                        this.moveTimeCounter = 0;
                        //Moves the player back so that they only traveled the distance that they should have
                        this.transform.position -= new Vector3((this.currentDistTraveled - this.distToMove), 0, 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    //Corrects any offsets that made the player not move far enough
                    else if (this.currentDistTraveled < this.distToMove && this.moveTimeCounter <= 0)
                    {
                        //Moves the player forward so they finish traveling the remaining distance
                        this.transform.position += new Vector3((this.distToMove - this.currentDistTraveled), 0, 0);

                        //Resets the distance currently traveled
                        this.currentDistTraveled = 0;
                    }
                    break;
            }
        }

        //If any of the counters are counting down, no action can be taken
        if (this.changeDirectionCounter > 0 || this.moveTimeCounter > 0)
            return;

        //Check to see which direction the player wants to move
        switch (this.GetDirectionInput())
        {
            //If there's no player input, nothing happens
            case Directions.None:
                this.TrackInput(false);
                break;

            //If they want to move Up
            case Directions.Up:
                //Check if we're already looking up
                if (this.directionFacing == Directions.Up)
                {
                    //If so, check to see if the tile we're looking at is solid (we aren't a ghost...)
                    if (this.CheckIfTileIsEmpty(Directions.Up))
                    {
                        this.moveTimeCounter = this.timeToMove;
                    }
                    //If the tile isn't free, plays a collision sound so the player knows they can't move
                    else
                    {
                    }

                    //Sets it so that the player registered as "moving" so there will be no delay on movement if they change direction
                    this.TrackInput(true);
                }
                //If we aren't looking Up, now we are
                else
                {
                    this.SetNewDirectionLooking(Directions.Up);
                }
                break;

            //If they want to move Down
            case Directions.Down:
                //Check if we're already looking down
                if (this.directionFacing == Directions.Down)
                {
                    //If so, check to see if the tile we're looking at is solid (we aren't a ghost...)
                    if (this.CheckIfTileIsEmpty(Directions.Down))
                    {
                        this.moveTimeCounter = this.timeToMove;
                    }
                    //If the tile isn't free, plays a collision sound so the player knows they can't move
                    else
                    {
                    }

                    //Sets it so that the player registered as "moving" so there will be no delay on movement if they change direction
                    this.TrackInput(true);
                }
                //If we aren't looking Down, now we are
                else
                {
                    this.SetNewDirectionLooking(Directions.Down);
                }
                break;

            //If they want to move Left
            case Directions.Left:
                //Check if we're already looking Left
                if (this.directionFacing == Directions.Left)
                {
                    //If so, check to see if the tile we're looking at is solid
                    if (this.CheckIfTileIsEmpty(Directions.Left))
                    {
                        this.moveTimeCounter = this.timeToMove;
                    }
                    //If the tile isn't free, plays a collision sound so the player knows they can't move
                    else
                    {
                    }

                    //Sets it so that the player registered as "moving" so there will be no delay on movement if they change direction
                    this.TrackInput(true);
                }
                //If we aren't looking Left, now we are
                else
                {
                    this.SetNewDirectionLooking(Directions.Left);
                }
                break;

            //If they want to move Right
            case Directions.Right:
                //Check if we're already looking right
                if (this.directionFacing == Directions.Right)
                {
                    //If so, check to see if the tile we're looking at is solid
                    if (this.CheckIfTileIsEmpty(Directions.Right))
                    {
                        this.moveTimeCounter = this.timeToMove;
                    }
                    //If the tile isn't free, plays a collision sound so the player knows they can't move
                    else
                    {
                    }

                    //Sets it so that the player registered as "moving" so there will be no delay on movement if they change direction
                    this.TrackInput(true);
                }
                //If we aren't looking Right, now we are
                else
                {
                    this.SetNewDirectionLooking(Directions.Right);
                }
                break;
        }
    }


    //Function called from Update. Returns which direction the player is indicating to move
    private Directions GetDirectionInput()
    {
        //The direction that this function returns
        Directions returnDirection = Directions.None;

        //If either of the UP keys are pressed and none of the others are, returns UP
        if (Input.GetKey(this.upKey) || Input.GetKey(this.upKeyAlt)
            && !Input.GetKey(this.downKey) && !Input.GetKey(this.downKeyAlt)
            && !Input.GetKey(this.leftKey) && !Input.GetKey(this.leftKeyAlt)
            && !Input.GetKey(this.rightKey) && !Input.GetKey(this.rightKeyAlt))
        {
            returnDirection = Directions.Up;
        }
        //If either of the DOWN keys are pressed and none of the others are, returns DOWN
        else if (Input.GetKey(this.downKey) || Input.GetKey(this.downKeyAlt)
            && !Input.GetKey(this.upKey) && !Input.GetKey(this.upKeyAlt)
            && !Input.GetKey(this.leftKey) && !Input.GetKey(this.leftKeyAlt)
            && !Input.GetKey(this.rightKey) && !Input.GetKey(this.rightKeyAlt))
        {
            returnDirection = Directions.Down;
        }
        //If either of the LEFT keys are pressed and none of the others are, returns LEFT
        else if (Input.GetKey(this.leftKey) || Input.GetKey(this.leftKeyAlt)
            && !Input.GetKey(this.downKey) && !Input.GetKey(this.downKeyAlt)
            && !Input.GetKey(this.upKey) && !Input.GetKey(this.upKeyAlt)
            && !Input.GetKey(this.rightKey) && !Input.GetKey(this.rightKeyAlt))
        {
            returnDirection = Directions.Left;
        }
        //If either of the RIGHT keys are pressed and none of the others are, returns RIGHT
        else if (Input.GetKey(this.rightKey) || Input.GetKey(this.rightKeyAlt)
            && !Input.GetKey(this.downKey) && !Input.GetKey(this.downKeyAlt)
            && !Input.GetKey(this.leftKey) && !Input.GetKey(this.leftKeyAlt)
            && !Input.GetKey(this.upKey) && !Input.GetKey(this.upKeyAlt))
        {
            returnDirection = Directions.Right;
        }

        return returnDirection;
    }


    //Function called from Update. Returns true of the location in the given direction is empty. Used to check if we can move into a tile
    private bool CheckIfTileIsEmpty(Directions directionLooking_ = Directions.None)
    {
        //Does nothing if no direction is given. This should NEVER happen, so we send out an error message
        if (directionLooking_ == Directions.None)
        {
            Debug.LogError("ERROR: PlayerMovementLogic.CheckIfTileIsEmpty, No direction indicated to check");
            return false;
        }

        //The start point and direction where we'll be raycasting to check for colliders
        Vector2 startLoc = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 direction = new Vector2(0, 0);

        //Alters the position of the end location based on the direction the player is looking
        switch (directionLooking_)
        {
            case Directions.Up:
                direction = new Vector2(0, 1);
                break;

            case Directions.Down:
                direction = new Vector2(0, -1);
                break;

            case Directions.Left:
                direction = new Vector2(-1, 0);
                break;

            case Directions.Right:
                direction = new Vector2(1, 0);
                break;
        }

        //Casts the raycast using the player's position and the direction they're facing
        RaycastHit2D raycastResults = Physics2D.Raycast(startLoc, direction, this.distToMove);
        if (raycastResults.collider != null)
        {
            //If something's in the way, the space is occupied and we can't move there
            return false;
        }

        //Returns true (direction is empty) if nothing else indicates otherwise
        return true;
    }


    //Function that helps to track if the player moved during the last 3 frames. Input: true if the player moved this frame
    private void TrackInput(bool movedThisFrame_ = false)
    {
        //Shifts the 2nd to last frame to the 3rd to last
        this.wasMovingLastFrame[2] = this.wasMovingLastFrame[1];

        //Shifts the last frame to the 2nd to last
        this.wasMovingLastFrame[1] = this.wasMovingLastFrame[0];

        //Tracks the newest frame
        this.wasMovingLastFrame[0] = movedThisFrame_;
    }


    //Function called from Update. Sets this player to be looking at the new direction and changes their sprites
    private void SetNewDirectionLooking(Directions newDirection_ = Directions.None)
    {
        //Does nothing if there's no new direction to face
        if (newDirection_ == Directions.None)
            return;

        //Saves the direction
        this.directionFacing = newDirection_;

        //Changes this player's sprite to match the direction
        Debug.LogWarning("WARNING: PlayerMovementLogic.SetNewDirectionLooking: Add in sprite changing functionality!");

        //Right now, we're just rotating the sprite of this object for testing purposes
        switch (this.directionFacing)
        {
            case Directions.Up:
                this.transform.eulerAngles = new Vector3(0, 0, 90);
                break;

            case Directions.Down:
                this.transform.eulerAngles = new Vector3(0, 0, 270);
                break;

            case Directions.Left:
                this.transform.eulerAngles = new Vector3(0, 0, 180);
                break;

            case Directions.Right:
                this.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }

        //If the player was standing still before changing directions, there's a slight delay before moving
        if (!this.wasMovingLastFrame[0] && !this.wasMovingLastFrame[1] && !this.wasMovingLastFrame[2])
        {
            //Starts the counter for changing directions so that there's a delay between looking a direction and moving
            this.changeDirectionCounter = this.changeDirectionDelay;
        }
    }
}

public enum Directions
{
    None,
    Up,
    Down,
    Left,
    Right
}