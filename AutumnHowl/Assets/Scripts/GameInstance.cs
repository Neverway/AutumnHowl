//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//  Erriney
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private static GameInstance instance;

    
    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance);
    }

    
    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Directly gets a component from the GameInstance instance of the type provided
    /// </summary>
    /// <typeparam name="T">GameInstance component you wish to retrieve</typeparam>
    /// <returns>The component of type T from GameInstance</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static T Get<T>() where T : MonoBehaviour
    {
        if (instance == null)
            throw new NullReferenceException($"Trying to get GameInstance component, but there is no GameInstance. " +
                                             $"(or it is not stored in {nameof(GameInstance)}.{nameof(instance)}");

        return instance.GetComponent<T>();
    }


    #endregion
}
