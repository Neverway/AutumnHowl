using UnityEngine;

public class OverworldEnemySpawner : AutoGUIDObjectRecreator<OverworldEnemy>, ICreatesGameObject
{
    GameObject ICreatesGameObject.GetCreatedGameObject()
    {
        OverworldEnemy enemy = CreateNew();
        enemy.OnNewInstance();
        enemy.homeCycle = GameInstance.Gamestate.currentCycle;
        return enemy.gameObject;
    }
    //int ICreatesGameObject.GetSeed() => GameInstance.Gamestate.GetCycleSubSeed(GetGUID());
}
