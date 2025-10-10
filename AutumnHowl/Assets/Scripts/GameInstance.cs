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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameInstance : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/

    public static InputActions.TopDownActions Inputs { get; private set; }

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
    private void OnEnable()
    {
        Inputs = new InputActions().TopDown;
        Inputs.Enable();
    }
    private void OnDisable()
    {
        if (this == instance)
            Inputs.Disable();
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

    public static void SendCoroutine(IEnumerator coroutine)
    {
        instance.StartCoroutine(coroutine);
    }

    #endregion
}
