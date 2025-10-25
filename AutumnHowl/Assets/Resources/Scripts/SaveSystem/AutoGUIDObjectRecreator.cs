using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoGUIDObjectRecreator<TRecreatable> : AutoGUIDObject<string[]>
    where TRecreatable : AutoGUIDObjectBase
{
    public TRecreatable recreatablePrefab;
    private List<TRecreatable> recreatables = new List<TRecreatable>();

    public TRecreatable CreateNew()
    {
        byte[] unityRandomGuidBytes = new byte[16];

        for (int i = 0; i <  unityRandomGuidBytes.Length; i++)
            unityRandomGuidBytes[i] = (byte)Random.Range(0, 256);

        return CreateNewWithGUID(new System.Guid(unityRandomGuidBytes).ToString());
    }
    public TRecreatable CreateNewWithGUID(string newGUID)
    {
        string oldGuid = recreatablePrefab.GetGUID();
        recreatablePrefab.SetGUID(newGUID);
        TRecreatable newObject = Instantiate(recreatablePrefab);
        newObject.SetGUID(newGUID);
        recreatablePrefab.SetGUID(oldGuid);
        recreatables.Add(newObject);
        return newObject;
    }

    public override string[] OnSaveInstance()
    {
        //Filter out all null (destroyed) objects
        recreatables = recreatables.Where(obj => obj != null).ToList();
        //Convert list of recreateables into an array of their guids
        string[] currentGuids = recreatables.Select(obj => obj.GetGUID()).ToArray();
        
        if (HasData())
        {
            //Get old list of saved guids
            string[] oldSavedGuids = GetData();
            //Remove save data for old guids that dont exist in current guids
            foreach (string oldGuid in oldSavedGuids)
                if (!currentGuids.Contains(oldGuid))
                    AutoGUIDListener.ClearSavedData(oldGuid);
        }
        return currentGuids;
    }
    public override void OnLoadInstance(string[] guidsToLoad)
    {
        //Filter out all null (destroyed) objects
        recreatables = recreatables.Where(obj => obj != null).ToList();
        List<TRecreatable> newRecreateables = new List<TRecreatable>();
        foreach (string guidToLoad in guidsToLoad)
        {
            bool loaded = false;
            for (int i = 0; i < recreatables.Count; i++)
            {
                if (recreatables[i].GetGUID() == guidToLoad)
                {
                    loaded = true;
                    break;
                }
            }
            if (!loaded)
            {
                TRecreatable newRecreateable = CreateNewWithGUID(guidToLoad);
                newRecreateable.ForceLoad();
                newRecreateables.Add(newRecreateable);
            }
        }
        foreach (var recreateable in recreatables.ToArray())
        {
            string guid = recreateable.GetGUID();
            if (!guidsToLoad.Contains(guid))
            {
                AutoGUIDListener.ClearSavedData(guid);
                Destroy(recreateable.gameObject);
            }
        }
        //recreatables = recreatables.Where(obj => obj != null).ToList();
        recreatables.AddRange(newRecreateables);
    }
    public override void OnNewInstance()
    {

    }
}
