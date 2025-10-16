using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveDataStrategy
{
    public abstract void SaveValue<T>(T value, string id);
    public abstract T LoadValue<T>(T defaultValue, string id);
}
