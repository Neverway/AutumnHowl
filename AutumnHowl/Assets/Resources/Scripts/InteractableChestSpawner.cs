using System.Linq;
using UnityEngine;

public class InteractableChestSpawner : AutoGUIDObjectRecreator<InteractableChest>, ICreatesGameObject
{
    public LootTable[] cycleLootTables;
    public GameObject GetCreatedGameObject()
    {
        InteractableChest chest =  CreateNew();
        int cycle = GameInstance.Gamestate.currentCycle;
        if (cycleLootTables.IsIndexOutOfRange(cycle))
            cycle = cycleLootTables.Count() - 1;

        if (cycle == -1)
        {
            Debug.LogError("There was no loot tables assigned to chest spawner " +
                "<color=grey>(Click to jump to spawner)</color>", this);
            return chest.gameObject;
        }

        chest.itemsToGive = new ItemsRef_LootTable() { lootTable = cycleLootTables[cycle] } ;
        chest.GenerateChestContents();

        return chest.gameObject;
    }
}
