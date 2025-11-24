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

public class LB_Generic : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Tooltip("When enabled, the currently playing music track will not be overridden")]
    public bool turnOffMusicSelection = false;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [Tooltip("The music track to begin playing when this level loads")]
    public GI_AudioManager.Music musicTrack = GI_AudioManager.Music.none;
    
    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        GameInstance.Get<GI_TransitionManager>().Fadein();
        if (turnOffMusicSelection == false) StartCoroutine(StartMusicRoutine());
    }

    private IEnumerator StartMusicRoutine ()
    {
        yield return new WaitUntil (() => GI_AudioManager.Instance != null);
        GI_AudioManager.Instance.SetMusic (musicTrack);
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
