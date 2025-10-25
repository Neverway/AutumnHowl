using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class IDToObj<T>
{
    public static Dictionary<string, T> idToInstance;

    public static void ClearNew() => idToInstance = new();
    public static bool TryAdd(string id, T obj)
    {
        return idToInstance.TryAdd(id, obj);
    }
    public static bool TryGet(string id, out T obj)
    {
        if (idToInstance == null)
        {
            Debug.Log("was empty");
            obj = default(T);
            return false;
        }
        return idToInstance.TryGetValue(id, out obj);
    }

    public static void AddAllFromUnityResources(string subfolders = "", bool clearOldObjs = true, bool warnAboutEmptyIDs = true)
    {
        if (!typeof(Object).IsAssignableFrom(typeof(T)))
        {
            Exception e = new ArgumentException(
                $"Cannot call IDToObj<{typeof(T).Name}>.AddAllFromUnityResources() on non-UnityEngine.Object types");
            Debug.LogException(e);
            throw e;
        }

        if (!typeof(UniquelyIdentifiable).IsAssignableFrom(typeof(T)))
        {
            Exception e = new ArgumentException(
                $"Type must be {nameof(UniquelyIdentifiable)} if you want to call IDToObj<{typeof(T).Name}>.AddAllFromUnityResources()");
            Debug.LogException(e);
            throw e;
        }

        if (clearOldObjs)
            ClearNew();

        foreach (Object obj in Resources.LoadAll(subfolders, typeof(T)))
        {
            string ID = (obj as UniquelyIdentifiable).UniqueID;

            if (string.IsNullOrEmpty(ID))
            {
                if (warnAboutEmptyIDs)
                    Debug.LogWarning($"{typeof(T).Name} has no assigned UniqueID on {obj.name}, cannot add to IDToObj lookup " +
                        $"<color=grey>(Click to highlight)</color>", obj);
                continue;
            }

            if (obj is T resource)
            {
                if (!TryAdd(ID, resource))
                {
                    Exception e = new ArgumentException(
                        $"Duplicate UniqueID ({ID}) for {typeof(T).Name} on {obj} and {idToInstance[ID]}");
                    Debug.LogException(e, obj);
                    throw e;
                }
            }
        }
    }
}

public interface UniquelyIdentifiable
{
    public string UniqueID { get; }
}
