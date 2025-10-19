using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveDataStrategy
{
    public abstract void Save(string fileName);
    public abstract void Load(string fileName);

    public abstract void WriteValue<T>(T value, string id);
    public abstract T ReadValue<T>(T defaultValue, string id);

}
