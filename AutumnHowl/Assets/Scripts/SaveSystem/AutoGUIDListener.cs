using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface AutoGUIDListener
{
    //Inhereted Methods ----------------------------------------------------------------------------------------------------------
    public abstract string OnSaveGUID();
    public abstract void OnLoadGUID(string loadData);
    public abstract void OnLoadGUID_NoData();


    //Stored information for GUID instances and GUID Save Data ----------------------------------------------------------------------
    [SaveAndLoadProperty("GUIDInstanceSaveData")]
    public static SerializableDictionary<string, string> GUIDSaveData { get; set; }
    public static Dictionary<string, AutoGUIDListener> currentGUIDInstances;


    

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 
    private static void InitializeDictionaries()
    {
        currentGUIDInstances = new();
        GUIDSaveData = new();
    }

    //GUID Instance registering ------------------------------------------------------------------------------------------------------
    public static void RegisterGuidInstance(GUIDComponent guid, AutoGUIDListener instance)
    {
        if (guid == null || string.IsNullOrEmpty(guid.GetGUID()))
        {
            Debug.LogError("Cannot register GUID that does not exist DUMMY", guid);
            throw new System.NullReferenceException();
        }
        Debug.Log("Yippie! I registered!: " + guid.GetGUID(), guid);
        currentGUIDInstances.Add(guid.GetGUID(), instance);
    }
    public static void UnregisterGuidInstance(GUIDComponent guid)
        => currentGUIDInstances.Remove(guid.GetGUID());

    //Helper Methods ------------------------------------------------------------------------------------------------------------------
    public static bool ClearSavedData(GUIDComponent guid) => GUIDSaveData.Remove(guid.GetGUID());
    public static bool ClearSavedData(string guid) => GUIDSaveData.Remove(guid);
    public static void ForceSave(AutoGUIDObjectBase guidObject)
    {
        string guid = guidObject.GetGUID();
        if (!GUIDSaveData.ContainsKey(guid))
            GUIDSaveData.Add(guid, guidObject.OnSaveGUID());
        else
            GUIDSaveData[guid] = guidObject.OnSaveGUID();
    }
    public static void ForceLoad(AutoGUIDObjectBase guidObject)
    {
        if (GUIDSaveData.TryGetValue(guidObject.GetGUID(), out string loadData))
            guidObject.OnLoadGUID(loadData);
        else
            guidObject.OnLoadGUID_NoData();
    }
    public static string GetSavedData(GUIDComponent guidObject)
    {
        string guid = guidObject.GetGUID();
        if (GUIDSaveData.ContainsKey(guid))
            return GUIDSaveData[guid];
        return null;
    }
    public static bool HasSavedData(GUIDComponent guidObject) 
        => GUIDSaveData.ContainsKey(guidObject.GetGUID());

    //Save and Load callbacks ---------------------------------------------------------------------------------------------------------
    [InvokeBeforeSave]
    private static void OnGameSave()
    {
        //Save data on each registered guidInstance to GUIDSaveData
        foreach (var guidInstance in currentGUIDInstances.ToArray())
        {
            if (!GUIDSaveData.ContainsKey(guidInstance.Key))
                GUIDSaveData.Add(guidInstance.Key, guidInstance.Value.OnSaveGUID());
            else
                GUIDSaveData[guidInstance.Key] = guidInstance.Value.OnSaveGUID();
        }
    }
    [InvokeAfterLoad]
    private static void OnGameLoad()
    {
        //Make sure GUIDSaveData list is not null, just for avoiding errors
        if (GUIDSaveData == null) { GUIDSaveData = new(); return; }

        //Load data from GUIDSaveData to each registered guidInstance
        foreach (var guidInstance in currentGUIDInstances.ToArray())
        {
            if (GUIDSaveData.TryGetValue(guidInstance.Key, out string data))
                guidInstance.Value.OnLoadGUID(data);
            else
                guidInstance.Value.OnLoadGUID_NoData();
        }
    }
}