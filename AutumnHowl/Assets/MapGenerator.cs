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

    private int mapWidth = 5;
    private int mapHeight = 5;
    private MapNode[,] mapNodes;

    public const int directionCount = 4;

    //=-----------------=
    // Reference Variables
    //=-----------------=

    [SerializeField] private RuleTile groundTile;
    [SerializeField] private GameObject[] decor;

    //=-----------------=
    // Mono Functions
    //=-----------------=
    private void Start()
    {
        GenerateMap();
    }

    private void Update()
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
                MapNode n = mapNodes[x,y];
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
        Debug.Log(p);
    }

    private void GenerateMap ()
    {
        mapNodes = new MapNode[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight;y++)
        {
            for (int x = 0; x < mapWidth;x++)
            {
                mapNodes[x, y] = new MapNode();
            }
        }
        GenerateFromNode (mapWidth / 2, 0);
        Debug.Log ("Map Nodes Finished");
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
        if (IsNodeWalkable(x, y - 1))
        {
            possibleNodes++;
            foundPaths[0] = true;
        }
        if (IsNodeWalkable(x, y + 1))
        {
            possibleNodes++;
            foundPaths[1] = true;
        }
        if (IsNodeWalkable (x-1, y))
        {
            possibleNodes++;
            foundPaths[2] = true;
        }
        if (IsNodeWalkable (x+1,y))
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
        for (int i = 0;i < 4;i++)
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

    //=-----------------=
    // External Functions
    //=-----------------=
}

class MapNode
{
    public bool[] paths;
    public bool visited = false;

    public MapNode  ()
    {
        paths = new bool[MapGenerator.directionCount];
    }
}