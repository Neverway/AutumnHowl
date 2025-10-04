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

public class WB_Shop : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private IEnumerator ExitCoroutine()
    {
        GameInstance.Get<GI_TransitionManager>().Fadeout();
        yield return new WaitForSeconds(0.5f);
        GameInstance.Get<GI_WorldLoader>().Load("Town", "Shop");
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void Exit()
    {
        StartCoroutine(ExitCoroutine());
    }


    #endregion
}
