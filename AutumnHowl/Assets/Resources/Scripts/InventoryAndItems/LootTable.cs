using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Item/New Loot Table", fileName = "loottable_")]
public class LootTable : ScriptableObject
{
    [TextArea(1,50)] public string notes;

    [Box, Polymorphic, SerializeReference] 
    public ItemsReference itemsToGrant;
    public Item[] GetLoot() => itemsToGrant.GetItems();
}