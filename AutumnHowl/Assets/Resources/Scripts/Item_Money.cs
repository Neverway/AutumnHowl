using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>The only purpose of this is just to represent money as an item</summary>
public class Item_Money : Item
{
    public int moneyValue;
    protected override bool OnUse(CharacterIdentifier user, int _atIndex, int _inList = 0) => false;
    public override string GetDescription() => $"{("{col=stat}")}[+{moneyValue}$]";
    public void GetMoney()
    {
        GameInstance.Gamestate.money += moneyValue;
        Destroy(this);
    }

    public static Item_Money CreateNew(int money_amount)
    {
        Item_Money money_item = CreateInstance<Item_Money>();

        money_item.moneyValue = money_amount;
        money_item.displayName = "{col=stat}" + money_amount + "${col=}";
        money_item.UniqueID = money_item.ConvertToDataString();

        return money_item;
    }

    public string ConvertToDataString() => "$" + moneyValue;
    public static bool TryConvertDataStringToMoney(string input, out Item_Money money)
    {
        money = null;

        //Get rid of empty spaces or special characters and required "$" sign in string
        if (string.IsNullOrEmpty(input) || !input.Contains('$')) return false;
        input = input.Trim(' ', '\t', '\n', '\r', '$');

        //Try to parse trimmed string as an int for money amount
        if (!int.TryParse(input, out int money_amount)) return false;
        money = CreateNew(money_amount);

        return true;
    }
}
