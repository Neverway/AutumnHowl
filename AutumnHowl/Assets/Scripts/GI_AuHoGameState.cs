//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GI_AuHoGameState : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public AuHoGameState currentGameState;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Character player;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}

[Serializable]
public class AuHoGameState
{
    public string map = "Town";
    public Vector2 overworldPosition;
    public CharacterStats playerStats = new CharacterStats();
    public int money = 0;
    public Inventory inventory = new Inventory();
    public int kills = 0;
    public int deaths = 0;
    public float playtime = 0;
    public BattleData currentBattle;
}

[Serializable]
public class UsingEffect
{
    public Effect effect;
    public Affected affected;
    public float amount;
}

[Serializable]
public enum Effect
{
    none,
    heal,
    damage,
    corrupt,
    attack,
    strength,
    defense,
}

[Serializable]
public enum Affected
{
    none,
    user,
    target,
    all,
}