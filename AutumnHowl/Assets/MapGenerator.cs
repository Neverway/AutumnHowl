//===================== (Neverway 2024) Written by Liz M. =====================
//
// Purpose:
// Notes:
//
//=============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    //=-----------------=
    // Public Variables
    //=-----------------=


    //=-----------------=
    // Private Variables
    //=-----------------=

    [SerializeField] private int mapWidth = 5;
    [SerializeField] private int mapHeight = 5;
    private MapNode[,] mapNodes;

    public const int directionCount = 4;

    [SerializeField] private int roomWidth = 10;
    [SerializeField] private int roomHeight = 8;

    [SerializeField] private int pathWidth = 3;
    private int pathRadius;
    [SerializeField] private float pathWidthRandomness = 2;

    //=-----------------=
    // Reference Variables
    //=-----------------=

    [SerializeField] private Tilemap tilemapGround;
    [SerializeField] private RuleTile groundTile;
    [SerializeField] private Tilemap tilemapCollision;
    [SerializeField] private Tile collisionTile;
    [SerializeField] private Tile emptyTile;
    [SerializeField] private GameObject[] propList;

    //=-----------------=
    // Mono Functions
    //=-----------------=
    private void Start ()
    {
        GenerateMap ();
    }

    private void Update ()
    {

    }

    //=-----------------=
    // Internal Functions
    //=-----------------=

    private void PrintNodes ()
    {
        string p = "Map:\n";
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                MapNode n = mapNodes[x, y];
                if (mapNodes[x, y].visited)
                {
                    if (n.paths[0] && !n.paths[1] && !n.paths[2] && !n.paths[3])
                    {
                        p += "╨";
                    }
                    if (!n.paths[0] && n.paths[1] && !n.paths[2] && !n.paths[3])
                    {
                        p += "╥";
                    }
                    if (!n.paths[0] && !n.paths[1] && n.paths[2] && !n.paths[3])
                    {
                        p += "╡";
                    }
                    if (!n.paths[0] && !n.paths[1] && !n.paths[2] && n.paths[3])
                    {
                        p += "╞";
                    }
                    if (n.paths[0] && n.paths[1] && !n.paths[2] && !n.paths[3])
                    {
                        p += "║";
                    }
                    if (!n.paths[0] && !n.paths[1] && n.paths[2] && n.paths[3])
                    {
                        p += "═";
                    }
                    if (n.paths[0] && !n.paths[1] && n.paths[2] && !n.paths[3])
                    {
                        p += "╝";
                    }
                    if (n.paths[0] && !n.paths[1] && !n.paths[2] && n.paths[3])
                    {
                        p += "╚";
                    }
                    if (!n.paths[0] && n.paths[1] && n.paths[2] && !n.paths[3])
                    {
                        p += "╗";
                    }
                    if (!n.paths[0] && n.paths[1] && !n.paths[2] && n.paths[3])
                    {
                        p += "╔";
                    }
                    if (n.paths[0] && n.paths[1] && n.paths[2] && n.paths[3])
                    {
                        p += "╬";
                    }
                    if (!n.paths[0] && n.paths[1] && n.paths[2] && n.paths[3])
                    {
                        p += "╦";
                    }
                    if (n.paths[0] && !n.paths[1] && n.paths[2] && n.paths[3])
                    {
                        p += "╩";
                    }
                    if (n.paths[0] && n.paths[1] && !n.paths[2] && n.paths[3])
                    {
                        p += "╠";
                    }
                    if (n.paths[0] && n.paths[1] && n.paths[2] && !n.paths[3])
                    {
                        p += "╣";
                    }
                }
                else
                {
                    p += "░";
                }
            }
            p += "\n";
        }
        Debug.Log (p);
    }

    private void GenerateMap ()
    {
        mapNodes = new MapNode[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                mapNodes[x, y] = new MapNode ();
            }
        }
        GenerateFromNode (mapWidth / 2, 0);
        Debug.Log ("Map Nodes Finished");
        GenerateTilesFromNodes ();
        Debug.Log ("Map Tiles Finished");
        ScatterTrees ();
    }

    private void ScatterTrees ()
    {
        if (propList.Length == 0)
        {
            Debug.LogError ("map generator lacks decor objects");
        }
        for (int x = 0; x <mapWidth * roomWidth; x+=2)
        {
            for (int y = 0; y< mapHeight * roomHeight; y+=2)
            {
                if (tilemapCollision.GetTile (new Vector3Int (x, y, 0)) == collisionTile)
                {
                    PlaceProp (x, y);
                }
            }
        }
    }

    private void PlaceProp (float x, float y)
    {
        GameObject prop = Instantiate (propList[UnityEngine.Random.Range(0,propList.Length)]);
        prop.transform.position = new Vector3 (
            x + UnityEngine.Random.Range (-1f, 1f),
            y + UnityEngine.Random.Range (-1f, 1f),
            prop.transform.position.z);
    }

    private bool IsNodeWalkable (int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x >= mapWidth || y >= mapHeight) return false;
        if (mapNodes[x, y].visited) return false;
        return true;
    }

    private void GenerateFromNode (int x, int y)
    {
        PrintNodes ();
        var node = mapNodes[x, y];
        node.visited = true;
        int possibleNodes = 0;
        bool[] foundPaths = new bool[4];
        if (IsNodeWalkable (x, y - 1))
        {
            possibleNodes++;
            foundPaths[0] = true;
        }
        if (IsNodeWalkable (x, y + 1))
        {
            possibleNodes++;
            foundPaths[1] = true;
        }
        if (IsNodeWalkable (x - 1, y))
        {
            possibleNodes++;
            foundPaths[2] = true;
        }
        if (IsNodeWalkable (x + 1, y))
        {
            possibleNodes++;
            foundPaths[3] = true;
        }
        if (possibleNodes == 0)
        {
            //Dead End
            return;
        }

        int rand = UnityEngine.Random.Range (0, possibleNodes);
        int moveDirection = 0;
        int n = -1;
        //We're going to check foundPaths until
        //"n" is equal to our random number.
        //This selects a path at random.
        for (int i = 0; i < 4; i++)
        {
            if (foundPaths[i])
            {
                n++;
            }
            if (n == rand)
            {
                moveDirection = i;
                break;
            }
        }

        switch (moveDirection)
        {
            case 0:
                {
                    //north
                    node.paths[0] = true;
                    mapNodes[x, y - 1].paths[1] = true;
                    GenerateFromNode (x, y - 1);
                    break;
                }
            case 1:
                {
                    //south
                    node.paths[1] = true;
                    mapNodes[x, y + 1].paths[0] = true;
                    GenerateFromNode (x, y + 1);
                    break;
                }
            case 2:
                {
                    //west
                    node.paths[2] = true;
                    mapNodes[x - 1, y].paths[3] = true;
                    GenerateFromNode (x - 1, y);
                    break;
                }
            case 3:
                {
                    //east
                    node.paths[3] = true;
                    mapNodes[x + 1, y].paths[2] = true;
                    GenerateFromNode (x + 1, y);
                    break;
                }
        }
        GenerateFromNode (x, y);
    }

    private void GenerateTilesFromNodes ()
    {
        pathRadius = pathWidth / 2;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                PlaceNodeTiles (x * roomWidth, y * roomHeight, mapNodes[x, y]);
            }
        }
    }


    private void PlaceNodeTiles (int xoffset, int yoffset, MapNode node)
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                tilemapCollision.SetTile(new Vector3Int(x+xoffset, y+yoffset), collisionTile);
            }
        }
        if (node.paths[0])
        {
            //north exit
            for (int y = 0; y < roomHeight / 2; y++)
            {
                SplatGround (roomWidth / 2 + xoffset, y + yoffset);
            }
        }
        if (node.paths[1])
        {
            //south exit
            for (int y = roomHeight / 2; y < roomHeight; y++)
            {
                SplatGround (roomWidth / 2 + xoffset, y + yoffset);
            }
        }
        if (node.paths[2])
        {
            //west exit
            for (int x = 0; x < roomWidth / 2; x++)
            {
                SplatGround (x + xoffset, roomHeight / 2 + yoffset);
            }
        }
        if (node.paths[3])
        {
            //east exit
            for (int x = roomWidth / 2; x < roomWidth; x++)
            {
                SplatGround (x + xoffset, roomHeight / 2 + yoffset);
            }
        }
    }

    private void SplatGround (int _x, int _y)
    {
        int collRadius = pathRadius + (int)(pathWidthRandomness/2);
        for (int x = -collRadius; x < collRadius; x++)
        {
            for (int y = -collRadius; y < collRadius; y++)
            {
                tilemapCollision.SetTile (new Vector3Int (_x + x, _y + y), null);
            }
        }

        float randomRadius = (float)pathRadius + UnityEngine.Random.Range (0f, pathWidthRandomness) - (pathWidthRandomness/2f);
        for (int x = -(int)randomRadius; x < (int)randomRadius; x++)
        {
            for (int y = -(int)randomRadius; y < (int)randomRadius; y++)
            {
                if (new Vector2(x,y).magnitude < randomRadius)
                {
                    tilemapGround.SetTile (new Vector3Int(_x + x, _y + y), groundTile);
                }
            }
        }
    }

    //=-----------------=
    // External Functions
    //=-----------------=
}

class MapNode
{
    public bool[] paths;
    public bool visited = false;

    public MapNode ()
    {
        paths = new bool[MapGenerator.directionCount];
    }
}
