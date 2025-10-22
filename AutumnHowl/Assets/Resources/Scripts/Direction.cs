using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
public enum Direction { North, East, South, West }
public enum SpinDirection { Left, Right, Spin180 }
/// <summary>Meant to contain information about each direction (defined in DirectionUtility)</summary>
public struct DirectionInfo
{
    //Information to provide in DirectionUtility constructor
    public float degreesRotation;
    public Vector2Int vector2;

    public Direction turnedLeft;
    public Direction turnedRight;
    public Direction turned180;

    public Func<bool> wasPressed;

    //More information (derived from input information)
    public int x => vector2.x;
    public int y => vector2.y;
}
//Define DirectionInfo for each direciton here
public static partial class DirectionUtility
{
    private static Dictionary<Direction, DirectionInfo> directionInfos;

    /// <summary>Called first time you reference this class, initializes directionInfo, setup all the info down below</summary>
    static DirectionUtility()
    {
        directionInfos = new Dictionary<Direction, DirectionInfo>
        {
            //Info for NORTH direction ------------------
            { Direction.North, new() {
                degreesRotation = 0f,
                vector2 = Vector2Int.up,

                turnedLeft = Direction.West,
                turnedRight = Direction.East,
                turned180 = Direction.South,

                wasPressed = () => GameInstance.Inputs.MoveUp.WasPressedThisFrame(),
            } },

            //Info for EAST direction -------------------
            { Direction.East, new() {
                degreesRotation = 90f,
                vector2 = Vector2Int.right,

                turnedLeft = Direction.North,
                turnedRight = Direction.South,
                turned180 = Direction.West,

                wasPressed = () => GameInstance.Inputs.MoveRight.WasPressedThisFrame(),
            } },

            //Info for SOUTH direction ------------------
            { Direction.South, new() {
                degreesRotation = 180f,
                vector2 = Vector2Int.down,

                turnedLeft = Direction.East,
                turnedRight = Direction.West,
                turned180 = Direction.North,

                wasPressed = () => GameInstance.Inputs.MoveDown.WasPressedThisFrame(),
            } },

            //Info for WEST direction -------------------
            { Direction.West, new() {
                degreesRotation = 270f,
                vector2 = Vector2Int.left,

                turnedLeft = Direction.South,
                turnedRight = Direction.North,
                turned180 = Direction.East,

                wasPressed = () => GameInstance.Inputs.MoveLeft.WasPressedThisFrame(),
            } }
        };
    }
}

/// <summary>Extention methods to easily get info about each direction</summary>
public static partial class DirectionUtility 
{
    public static DirectionInfo Info(this Direction direction) => directionInfos[direction];
    public static Direction Turn(this Direction direction, SpinDirection spin)
    {
        switch(spin)
        {
            case SpinDirection.Left: return directionInfos[direction].turnedLeft;
            case SpinDirection.Right: return directionInfos[direction].turnedRight;
            case SpinDirection.Spin180: return directionInfos[direction].turned180;
        }
        throw new NotImplementedException($"Did not define how to turn Directions with SpinDirection {spin}");
    }
    public static void InEachDireciton(Action<Direction> inEachDirection)
    {
        foreach (Direction direction in directionInfos.Keys)
            inEachDirection.Invoke(direction);
    }
    public static void InEachDireciton(Action<Direction, DirectionInfo> inEachDirection)
    {
        foreach (var dirInfos in directionInfos)
            inEachDirection.Invoke(dirInfos.Key, dirInfos.Value);
    }
    public static bool TryConvertToDirection(this Vector2 vector, out Direction? direction)
    {
        foreach (var dirInfos in directionInfos)
            if (dirInfos.Value.vector2 == vector)
            {
                direction = dirInfos.Key;
                return true;
            }
        direction = null;
        return false;
    }
}

