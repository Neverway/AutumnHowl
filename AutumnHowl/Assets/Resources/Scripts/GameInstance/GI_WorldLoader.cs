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
using UnityEngine.SceneManagement;

public class GI_WorldLoader : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    public bool IsLoading { get; private set; }


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private IEnumerator Co_Load(string _mapID, string _exitWarpID)
    {
        IsLoading = true;
        
        // Load the map
        var loadingMap = SceneManager.LoadSceneAsync(_mapID);
        while (loadingMap.isDone == false)
        {
            yield return null;
        }
        //If an exit to warp to has been given, teleport player there
        if (!string.IsNullOrEmpty(_exitWarpID))
            TeleportPlayerToExit(_exitWarpID);

        //Notify save system to load values for the scene
        GI_SaveSystem.NotifyEnteredScene();

        IsLoading = false;
    }



    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public bool TeleportPlayerToExit(string _exitWarpID)
    {
        // Get a reference to the game state (for the saved player position)
        var player = GameObject.FindGameObjectWithTag("Player");

        // Restore saved player position once loaded
        foreach (var warp in FindObjectsOfType<Volume_LevelChange>())
        {
            if (warp.warpExitID == _exitWarpID)
            {
                player.transform.root.position = warp.transform.position + warp.exitOffset;
                return true;
            }
        }
        return false;
    }

    public void Load(string _mapID, string _exitWarpID = null)
    {
        if (IsLoading) return;

        //Notify save system to save values from the scene you are leaving
        GI_SaveSystem.NotifyLeavingScene();

        //Start loading the next map
        GameInstance.Get<GI_AuHoGameState>().currentGameState.map = _mapID;
        StartCoroutine(Co_Load(_mapID, _exitWarpID));
    }


    #endregion
}
