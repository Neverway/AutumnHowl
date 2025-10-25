using UnityEngine;

public class OverworldEnemySpawner : AutoGUIDObjectRecreator<OverworldEnemy>, ICreatesGameObject
{
    public GameObject GetCreatedGameObject()
    {
        OverworldEnemy enemy = CreateNew();
        enemy.OnNewInstance();
        return enemy.gameObject;
    }
}
