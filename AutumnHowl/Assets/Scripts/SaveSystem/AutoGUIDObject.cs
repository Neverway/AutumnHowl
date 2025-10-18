using System;
using UnityEngine;

[Serializable]
public abstract class AutoGUIDObject<TData> : AutoGUIDObjectBase
{
    //Implementing GuidSavedInstance ------------------------------------------------------------------
    public override string OnSaveGUID() => DataToString(OnSaveInstance());
    public override void OnLoadGUID(string loadData) => OnLoadInstance(StringToData(loadData));
    public override void OnLoadGUID_NoData() => OnNewInstance();

    //abstract methods toImplement ---------------------------------------------------------------------
    public abstract TData OnSaveInstance();
    public abstract void OnLoadInstance(TData data);
    public abstract void OnNewInstance();

    //Conversion between TData and string ---------------------------------------------------------------
    private TData StringToData(string toConvert) => JsonUtility.FromJson<Wrapper<TData>>(toConvert).value;
    private string DataToString(TData toConvert) => JsonUtility.ToJson(new Wrapper<TData>(toConvert));
    [Serializable] public class Wrapper<T> { public T value; public Wrapper(T val) { value = val; } }

    //Helper Methods ---------------------------------------------------------------------------------
    public TData GetData() => StringToData(GetDataString());
}
[Serializable]
public abstract class AutoGUIDObjectBase : GUIDComponent, AutoGUIDListener
{
    //Register GUIDComponent with GuidSavedInstance system -------------------------------------------
    public virtual void OnEnable() => AutoGUIDListener.RegisterGuidInstance(this, this);
    public virtual void OnDisable() => AutoGUIDListener.UnregisterGuidInstance(this);

    //Implementing GuidSavedInstance (but not really, let the next class do it)  ---------------------
    public abstract string OnSaveGUID();
    public abstract void OnLoadGUID(string loadData);
    public abstract void OnLoadGUID_NoData();

    //Helper Methods ---------------------------------------------------------------------------------
    public void ForceLoad() => AutoGUIDListener.ForceLoad(this);
    public void ForceSave() => AutoGUIDListener.ForceSave(this);
    public string GetDataString() => AutoGUIDListener.GetSavedData(this);
    public bool HasData() => AutoGUIDListener.HasSavedData(this);
}