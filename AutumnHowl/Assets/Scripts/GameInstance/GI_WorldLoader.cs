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
    private bool isLoading;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private IEnumerator Co_Load(string _mapID, string _exitWarpID)
    {
        isLoading = true;
        
        // Load the map
        var loadingMap = SceneManager.LoadSceneAsync(_mapID);
        while (loadingMap.isDone == false)
        {
            yield return null;
        }
        
        // Get a reference to the game state (for the saved player position)
        var player = GameObject.FindGameObjectWithTag("Player");
        
        // Restore saved player position once loaded
        foreach (var warp in FindObjectsOfType<Volume_LevelChange>())
        {
            if (warp.warpExitID == _exitWarpID)
            {
                player.transform.root.position = warp.transform.position + warp.exitOffset;
            }
        }
        
        isLoading = false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void Load(string _mapID)
    {
        if (isLoading) return;
        GameInstance.Get<GI_AuHoGameState>().currentGameState.map = _mapID;
        SceneManager.LoadSceneAsync(_mapID);
    }
    
    public void Load(string _mapID, string _exitWarpID)
    {
        if (isLoading) return;
        GameInstance.Get<GI_AuHoGameState>().currentGameState.map = _mapID;
        StartCoroutine(Co_Load(_mapID, _exitWarpID));
    }


    #endregion
}
