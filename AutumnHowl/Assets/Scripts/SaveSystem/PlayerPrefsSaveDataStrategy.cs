using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsSaveDataStrategy : JsonBasedSaveDataStrategy
{
    protected override void SaveJsonValue(string value, string id) => PlayerPrefs.SetString(id, value);
    protected override string LoadJsonValue(string id) => PlayerPrefs.GetString(id, null);
}
