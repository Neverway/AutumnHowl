//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class func_clearHud : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public string[] widgetIDs;
    public float waitDuration;
    public BattleData battle;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void RemoveHudElement()
    {
        foreach (var widgetID in widgetIDs)
        {
            var target = GameInstance.Get<GI_WidgetManager>().GetExistingWidget(widgetID);
            if (target != null) Destroy(target);
        }
    }

    public void ReplaceHudElement()
    {
        foreach (var widgetID in widgetIDs)
        {
            GameInstance.Get<GI_WidgetManager>().AddWidget(widgetID);
        }
    }

    public void ClearMusic()
    {
        GameInstance.Get<GI_AudioManager>().SetMusic(GI_AudioManager.Music.none);
        StartCoroutine(WaitForAnimationToFinish());
    }

    public IEnumerator WaitForAnimationToFinish()
    {
        yield return new WaitForSeconds(waitDuration);
        GameInstance.Gamestate.EnterBattle(battle);
    }


    #endregion
}
