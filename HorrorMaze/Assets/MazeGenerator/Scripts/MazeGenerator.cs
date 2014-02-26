using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    //Variables
    public Vector3 StartingCoords = new Vector3(0, 0, 0);
    public int Width = 10;
    public int Height = 10;

    //Materials
    public GameObject OuterWalls;
    public GameObject Walls;
    public GameObject Ceiling;
    public GameObject Floors;

    //Booleans
    public bool GenerateMazeOnStartup = true;
    
    private int[,] _wallsToBePlaced;

    public int PassageWidth = 1;

    //Resolution
    public int ResolutionWidth = 2;
    public int ResolutionHeight = 2;

    //Directions
    private const int NORTH = 0;
    private const int EAST = 1;
    private const int SOUTH = 2;
    private const int WEST = 3;
    private const int NORTH_EAST = 4;
    private const int SOUTH_EAST = 5;
    private const int SOUTH_WEST = 6;
    private const int NORTH_WEST = 7;

    //Orientation
    private const int HORIZONTAL = 0;
    private const int VERTICAL = 1;

    //Holders
    private GameObject _floorHolder;
    private GameObject _wallHolder;
    private GameObject _ceilingHolder;

    //Sets
    private HashSet<String> _blacklist = new HashSet<string>();
    public List<float[]> Rooms = new List<float[]>();  

	// Use this for initialization
	void Start ()
	{
	    if (!OuterWalls) OuterWalls = Walls;

	    _floorHolder = GameObject.Find("FloorHolder");
	    _wallHolder = GameObject.Find("WallHolder");
        _ceilingHolder = GameObject.Find("CeilingHolder");

        if (GenerateMazeOnStartup)
        {
            LayFloorTiles();
            PlaceBorderWalls();
	        CreateMazeThroughRecusriveDivision();
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void PlaceTile(GameObject obj, float x, float y, float z, GameObject parent = null)
    {
        PlaceTile(obj, new Vector3(x * Floors.transform.localScale.x, y, z * Floors.transform.localScale.z), parent);
    }

    private void PlaceTile(GameObject obj, Vector3 coords, GameObject parent = null)
    {
        var t = (GameObject)Instantiate(obj, coords, obj.transform.rotation);
        t.gameObject.name = obj.name;
        if (parent != null)
        {
            t.transform.parent = parent.transform;
        }
    }

    private void LayFloorTiles()
    {
        if (Floors == null) return;
        for (var i = 0; i < Width; i++)
        {
            for (var k = 0; k < Height; k++)
            {
                PlaceTile(Floors, StartingCoords.x + i, StartingCoords.y, StartingCoords.z + k, _floorHolder);
                if (Ceiling == null) continue;
                PlaceTile(Ceiling, StartingCoords.x + i, StartingCoords.y + OuterWalls.transform.localScale.y + Ceiling.transform.localScale.y, StartingCoords.z + k, _ceilingHolder);
            }
        }
    }

    public List<Vector3> GetBorderWalls()
    {
        var borderWallList = new List<Vector3>();
        var yCoord = StartingCoords.y + (OuterWalls.transform.localScale.y / 2) + (Floors.transform.localScale.y / 2);
        for (int i = 0; i < Width; i++)
        {
            borderWallList.Add(new Vector3(i, yCoord, 0));
            borderWallList.Add(new Vector3(i, yCoord, Height - 1));
        }
        for (int i = 1; i < Height - 1; i++)
        {
            borderWallList.Add(new Vector3(0, yCoord, i));
            borderWallList.Add(new Vector3(Width - 1, yCoord, i));
        }
        return borderWallList;
    } 

    private void PlaceBorderWalls()
    {
        if (OuterWalls == null) return;
        GetBorderWalls().ForEach(wallCoord => PlaceTile(OuterWalls, wallCoord.x, wallCoord.y, wallCoord.z, _wallHolder));
    }

    private void CreateMazeThroughRecusriveDivision()
    {
        _wallsToBePlaced = new int[Width - 2, Height - 2];

        DivideGrid(new Vector2(0,0), Width-2, Height-2, ChooseDirectionToDivide(Width, Height));

        PlaceWalls();
    }

    private void DivideGrid(Vector2 startPoint, int width, int height, int orientation)
    {
        if (width - 1 <= ResolutionWidth || height - 1 <= ResolutionHeight)
        {
            var floatArray = new float[5];
            floatArray[0] = startPoint.x * Floors.transform.localScale.x;
            floatArray[1] = startPoint.y * Floors.transform.localScale.y;
            floatArray[2] = Floors.transform.localScale.z;
            floatArray[3] = width * Floors.transform.localScale.x;
            floatArray[4] = height * Floors.transform.localScale.y;
            Rooms.Add(floatArray);
            return;
        }
        var horizontal = (orientation == HORIZONTAL);
        var wallX = (int) (startPoint.x + (horizontal ? 0 : Mathf.Floor(Random.Range(1, width-2))));
        var wallY = (int) (startPoint.y + (horizontal ? Mathf.Floor(Random.Range(1, height-2)) : 0));
        var length = horizontal ? width : height;
        var direction = horizontal ? EAST : NORTH;
        var startCoords = new Vector2(wallX, wallY);
        //Debug.Log("HORIZONTAL? "+horizontal+"; Start Coords: (" + startCoords.x + ", " + startCoords.y + ")");

        
        DrawWallLine(startCoords,  length, direction);
        CutOutPassageFromWallLine(startCoords, length, direction);

        var newStartPoint = startPoint;
        if (direction == NORTH)
        {
            newStartPoint.y += length;
        }
        else
        {
            newStartPoint.x += length;
        }
        if (horizontal)
        {
            DivideGrid(startPoint, width, (int) (wallY - startPoint.y + 1), ChooseDirectionToDivide(wallX, wallY));
            DivideGrid(new Vector2(startPoint.x, wallY+1), width, (int) (startPoint.y+(height-wallY-1)), ChooseDirectionToDivide(wallX, wallY));
        }
        else
        {
            DivideGrid(startPoint, (int)(wallX - startPoint.x + 1), height, ChooseDirectionToDivide(wallX, wallY));
            DivideGrid(new Vector2(wallX+1, startPoint.y), (int)(startPoint.x+width-wallX-1), height, ChooseDirectionToDivide(wallX, wallY));
        }
        //DivideGrid(new Vector2(startCoords.x+1, startCoords.y+1), (int)(Width-2 - startCoords.x), (int)(Height-2 - startCoords.y), ChooseDirectionToDivide(wallX, wallY));
    }

    private void CutOutPassageFromWallLine(Vector2 start, int length, int direction)
    {
        var newCoords = start;
        if (direction == NORTH)
        {
            newCoords.y = Mathf.Floor(Random.Range(newCoords.y, length + newCoords.y));
        }
        else
        {
            newCoords.x = Mathf.Floor(Random.Range(newCoords.x, length + newCoords.x));
        }
        var tile = GetTile(newCoords);
        if ((int)tile.z == -1)
        {
            Debug.LogWarning("Tried to cut out a piece of passage that does not exist. Coord: (" + newCoords.x + ", " + newCoords.y + "); Old coord: ("+start.x+", "+start.y+")");
            return;
        }
        if ((int) tile.z == 0)
        {
            Debug.LogWarning("Tried to cut out a piece of passage that wasn't even a wall. Coord: (" + newCoords.x + ", " + newCoords.y + "); Old coord: (" + start.x + ", " + start.y + ")");
            return;
        }

        CutPassage(newCoords, direction);
    }

    private void CutPassage(Vector2 coords, int direction)
    {
        _wallsToBePlaced[(int)coords.x, (int)coords.y] = 0;
        BlacklistPassage(coords, direction);
        
// ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (PassageWidth > 1)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
        {
            var otherCoords = new Vector3(coords.x, coords.y, 0);
            var cutDirection = direction;
            for (var i = 0; i < PassageWidth; i++)
            {
                otherCoords = GetTile(new Vector2(otherCoords.x, otherCoords.y), cutDirection);
                if ((int)otherCoords.z == -1)
                {
                    cutDirection = ChangeDirection(cutDirection);
                    otherCoords.x = coords.x;
                    otherCoords.y = coords.y;
                    if (cutDirection == direction) return;
                    continue;
                }
                if ((int) otherCoords.z == 0)
                {
                    continue;
                }
                _wallsToBePlaced[(int)otherCoords.x, (int)otherCoords.y] = 0;
                BlacklistPassage(new Vector2(otherCoords.x, otherCoords.y), direction);
            }
        }
    }

    public int ChangeDirection(int currentDir)
    {
        if (currentDir == NORTH) return SOUTH;
        if (currentDir == SOUTH) return NORTH;
        if (currentDir == EAST) return WEST;
        if (currentDir == WEST) return EAST;
        if (currentDir == NORTH_EAST) return SOUTH_WEST;
        if (currentDir == NORTH_WEST) return SOUTH_EAST;
        if (currentDir == SOUTH_EAST) return NORTH_WEST;
        if (currentDir == SOUTH_WEST) return NORTH_EAST;
        return NORTH;
    }

    private void BlacklistPassage(Vector2 point, int perpendicularDirection)
    {
        _blacklist.Add(point.x+"-"+point.y);
        if (perpendicularDirection == EAST || perpendicularDirection == WEST)
        {
            var otherPoint = GetTile(point, NORTH);
            if ((int) otherPoint.z != -1)
                _blacklist.Add(otherPoint.x + "-" + otherPoint.y);
            otherPoint = GetTile(point, SOUTH);
            if ((int) otherPoint.z != -1)
                _blacklist.Add(otherPoint.x + "-" + otherPoint.y);
        }
        else
        {
            var otherPoint = GetTile(point, EAST);
            if ((int)otherPoint.z != -1)
                _blacklist.Add(otherPoint.x + "-" + otherPoint.y);
            otherPoint = GetTile(point, WEST);
            if ((int)otherPoint.z != -1)
                _blacklist.Add(otherPoint.x + "-" + otherPoint.y);
        }
    }

    private bool IsBlacklisted(Vector2 point)
    {
        return _blacklist.Contains(point.x + "-" + point.y);
    }

    private void DrawWallLine(Vector2 start, int length, int direction)
    {
        var tile = _wallsToBePlaced[(int)start.x, (int)start.y];
        var currentCoords = start;
        for (var i = 0; i < length; i++)
        {
            if (tile == -1)
            {
                Debug.LogWarning("Hit out of bounds exception before finished length: coords ("+ start.x + ", "+start.y + "); length: "+length + "; direction: " + direction+"; current coords: ("+currentCoords.x+", "+currentCoords.y+")");
                return;
            }
            if (tile == 1) continue;
            _wallsToBePlaced[(int)currentCoords.x, (int)currentCoords.y] = 1;
            var nextTile = GetTile(currentCoords, direction);
            tile = (int)nextTile.z;
            currentCoords = new Vector2(nextTile.x, nextTile.y);
        }
    }

    private int ChooseDirectionToDivide(int width, int height)
    {
        if (width > height) return HORIZONTAL; //0 == Horizontal
        if (height > width) return VERTICAL; //1 == Vertical
        return (int)Mathf.Floor(Random.Range(0, 2));
    }

    private void PlaceWalls()
    {
        var yCoord = StartingCoords.y + (Walls.transform.localScale.y/2) + (Floors.transform.localScale.y/2);
        for (var i = 0; i < Width-2; i++)
        {
            for (var k = 0; k < Height-2; k++)
            {
                if (_wallsToBePlaced[i, k] == 1)
                {
                    if (IsBlacklisted(new Vector2(i, k)))
                    {
                        continue;
                    }
                    PlaceTile(Walls, i + 1, yCoord, k + 1, _wallHolder);
                }
            }
        }
    }

    private Vector3 GetTile(Vector2 currentCoords, int direction = -1)
    {
        var returnVector = new Vector3(currentCoords.x, currentCoords.y, 0);
        switch (direction)
        {
            case NORTH: //North : 0
                returnVector.y += 1;
                break;
            case EAST: //East : 1
                returnVector.x += 1;
                break;
            case SOUTH: //South : 2
                returnVector.y -= 1;
                break;
            case WEST: //West : 3
                returnVector.x -= 1;
                break;
            case NORTH_EAST: //North-east : 4
                returnVector.y += 1;
                returnVector.x += 1;
                break;
            case SOUTH_EAST: //South-east : 5
                returnVector.y -= 1;
                returnVector.x += 1;
                break;
            case SOUTH_WEST: //South-West : 6
                returnVector.y -= 1;
                returnVector.x -= 1;
                break;
            case NORTH_WEST: //North-West : 7
                returnVector.y += 1;
                returnVector.x -= 1;
                break;
            default:
                break;
        }
        
        if (returnVector.x > Width - 3 || returnVector.y > Height - 3) returnVector.z = -1;
        else if (returnVector.x < 0 || returnVector.y < 0) returnVector.z = -1;
        else returnVector.z = _wallsToBePlaced[(int)returnVector.x, (int)returnVector.y];

        return returnVector;
    }
}
