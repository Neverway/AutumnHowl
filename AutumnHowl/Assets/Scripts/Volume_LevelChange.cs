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
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Volume_LevelChange : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    #if UNITY_EDITOR 
    public SceneAsset targetLevelScene;
    #endif
    public string targetLevel;
    
    [Tooltip("The id of the warp the player will be moved to when the next map loads. (Leave blank to not modify player pos on load)")]
    public string warpExitID;
    [Tooltip("When a player is moved to this warp position on load, this offset prevents re-triggers")]
    public Vector3 exitOffset;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Warp());
        }
    }

    private void OnValidate()
    {
#if UNITY_EDITOR 
        if (targetLevelScene == null) return;
        targetLevel = targetLevelScene.name;
#endif
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private IEnumerator Warp()
    {
        GameInstance.Get<GI_TransitionManager>().Fadeout();
        
        yield return new WaitForSeconds(0.5f);
        
        if (warpExitID == "")
        {
            GameInstance.Get<GI_WorldLoader>().Load(_mapID: targetLevel, _exitWarpID: warpExitID);
        }
        else
        {
            GameInstance.Get<GI_WorldLoader>().Load(_mapID: targetLevel);
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
