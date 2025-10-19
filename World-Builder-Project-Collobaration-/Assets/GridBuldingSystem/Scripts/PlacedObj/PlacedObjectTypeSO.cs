using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


/// <summary>
/// Defines data and utility methods for a placeable object type,  
/// including its prefab, size, orientation, and unit creation settings.
/// </summary>
[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject {

    public static Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:      return Dir.Left;
            case Dir.Left:      return Dir.Up;
            case Dir.Up:        return Dir.Right;
            case Dir.Right:     return Dir.Down;
        }
    }

    public enum Dir {
        Down,
        Left,
        Up,
        Right,
    }

    public enum PlacedObjectName
    {
        WaterLily,
        Bush,
        Mushroom,
        Tree_1,
        Tree_2,
        Tree_3

    }

    [Header("General")] 
    public PlacedObjectName placedObjectName;
    public int placedObjId;
    public Transform placedObjectPrefab;
    public Transform visualForGhostPlacedObject;
    [Range(1, 10)]
    public int maxAmountOfPlacedObjects; // maximum amount of placed objects of this type that can be created in the scene
    [Header("Grid Size")] 
    public int width;
    public int height;
    [Header("Unit Creation")]
    [SerializeField] bool canCreateUnit = true;
    [ShowIf(EConditionOperator.Or, "canCreateUnit")]
    public Transform unitToCreate;
    [ShowIf(EConditionOperator.Or, "canCreateUnit")]
    [Range(1, 10)]
    public int maxAmountOfUnits; // maximum amount of units that can be created for this placed object type



    public int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return 0;
            case Dir.Left:  return 90;
            case Dir.Up:    return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Left:  return new Vector2Int(0, width);
            case Dir.Up:    return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++) {
                    for (int y = 0; y < width; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

}
