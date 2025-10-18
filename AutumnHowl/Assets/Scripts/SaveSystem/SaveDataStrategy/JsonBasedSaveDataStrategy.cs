using UnityEngine;

public abstract class JsonBasedSaveDataStrategy : SaveDataStrategy
{
    public override void SaveValue<T>(T value, string id)
    {
        string stringValue = JsonUtility.ToJson(value);
        SaveJsonValue(stringValue, id);
    }
    public override T LoadValue<T>(T defaultValue, string id)
    {
        string stringValue = LoadJsonValue(id);
        if (stringValue == null) return defaultValue;
        return JsonUtility.FromJson<T>(stringValue);
    }
    public class Wrapper<T>
    {
        public Wrapper(T value) { this.value = value; }
        public T value;
    }
    protected abstract void SaveJsonValue(string value, string id);
    protected abstract string LoadJsonValue(string id);
}
