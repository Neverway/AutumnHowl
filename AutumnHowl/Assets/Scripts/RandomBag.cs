using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for grabbing things randomly from a "bag" such that you don't get repeats until the bag is emptied.
/// </summary>
public class RandomBag<T>
{
    //The list we will grab items from.
    private List<T> bag;
    private List<T> sourceList;

    public RandomBag(List<T> _sourceList) {
        if (_sourceList.Count == 0)
        {
            Debug.LogError ("Can't make bag from empty list");
            return;
        }
        sourceList = _sourceList;
        CreateBag();
    }

    public T Grab ()
    {
        if (bag == null || sourceList == null || sourceList.Count == 0)
        {
            Debug.LogError ("The bag had nothing in it. Returning default object.");
            return default(T);
        }
        int r = UnityEngine.Random.Range (0, bag.Count);
        T answer = bag[r];
        bag.RemoveAt (r);
        if (bag.Count == 0)
        {
            CreateBag();
        }
        return answer;
    }

    /// <summary>
    /// Rebuilds the Bag when it's out of items.
    /// </summary>
    /// <returns></returns>
    private void CreateBag ()
    {
        //Create the bag and clone the sourceList.
        bag = new List<T> ();
        foreach (T item in sourceList)
        {
            bag.Add (item);
        }
    }
}
