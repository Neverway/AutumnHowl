using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCycleHandler : MonoBehaviour
{
    public LootTable[] cycleShopItems;
    public WB_Shop wb_shop;

    public void UpdateShopItems()
    {
        int cycle = GameInstance.Gamestate.currentCycle;
        cycle--;

        if (cycleShopItems.IsIndexOutOfRange(cycle))
            cycle = cycleShopItems.Length - 1;

        Item[] shopItems = cycleShopItems[cycle].GetLoot(GameInstance.Gamestate.GetCycleSubSeed("ShopItems"));

        for (int i = 0; i < wb_shop.buyableItems.Count; i++)
        {
            if (shopItems.IsIndexInRange(i))
                wb_shop.buyableItems[i] = shopItems[i];
            else
                wb_shop.buyableItems[i] = null;
        }
    }
}
