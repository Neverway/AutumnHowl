using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JsonBasedSaveDataStrategy : SaveDataStrategy
{
    public override void SaveValue<T>(T value, string id)
    {
        string stringValue = JsonUtility.ToJson(new WrappedValue<T>(value));
        PlayerPrefs.SetString(id, stringValue);
    }
    public override T LoadValue<T>(T defaultValue, string id)
    {
        string stringValue = LoadJsonValue(id);
        if (stringValue == null)
            return defaultValue;

        return JsonUtility.FromJson<WrappedValue<T>>(stringValue).value;
    }
    private class WrappedValue<T>
    {
        public WrappedValue(T value) { this.value = value; }
        public T value;
    }

    protected abstract void SaveJsonValue(string value, string id);
    protected abstract string LoadJsonValue(string id);
}
