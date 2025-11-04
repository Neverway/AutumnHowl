using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleObstacleGenerator : MonoBehaviour
{
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;

    public List<GameObject> pawnsToPlace;

    public int numberOfObstacles;

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>BattleGrid.Instance!=null);
        //I really need this to execute after other objects have initialized. This is my jank solution. <3
        yield return new WaitForEndOfFrame();
        //make a list of empty grid tiles.
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int x = lowerBounds.x; x <= upperBounds.x; x++)
        {
            for (int y = lowerBounds.y; y <= upperBounds.y; y++)
            {
                if (!BattleGrid.Instance.IsOccupied(x, y))
                {
                    tiles.Add(new Vector2Int(x, y));
                }
            }
        }
        RandomBag<Vector2Int> tileBag = new RandomBag<Vector2Int>(tiles);
        RandomBag<GameObject> pawnBag = new RandomBag<GameObject>(pawnsToPlace);
        for (int i = 0; i < numberOfObstacles; i++) {
            BattleGrid.Instance.InstantiatePawn(tileBag.Grab(), pawnBag.Grab());
        }
    }
}
