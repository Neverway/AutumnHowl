using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemy : AutoGUIDObject<bool>
{
    public bool isDefeated = false;

    public override void OnLoadInstance(bool data) => isDefeated = data;
    public override void OnNewInstance() => isDefeated = false;
    public override bool OnSaveInstance() => isDefeated;
}
