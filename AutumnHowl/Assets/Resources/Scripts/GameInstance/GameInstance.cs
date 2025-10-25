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
[DefaultExecutionOrder(-10000)]
public class GameInstance : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Reload] private static GI_AuHoGameState _cachedGIGameState;
    public static AuHoGameState Gamestate => _cachedGIGameState.currentGameState;
    [Reload] public static Character Playerbody;
    public static InputActions.TopDownActions Inputs { get; private set; }


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private static GameInstance _instance;
    public static GameInstance Instance 
    { 
        get 
        {
            if (_instance == null) throw new NullReferenceException("GameInstance is missing");
            return _instance;
        } 
    }

    
    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        OnGameInstanceStart();
        DontDestroyOnLoad(_instance);
    }
    private void OnEnable()
    {
        Inputs = new InputActions().TopDown;
        Inputs.Enable();
    }
    private void OnDisable()
    {
        if (this == _instance)
            Inputs.Disable();
    }

    private void OnGameInstanceStart()
    {
        _cachedGIGameState = Get<GI_AuHoGameState>();
    }

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Directly gets a component from the GameInstance instance of the type provided
    /// </summary>
    /// <typeparam name="T">GameInstance component you wish to retrieve</typeparam>
    /// <returns>The component of type T from GameInstance</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static T Get<T>() where T : MonoBehaviour => Instance.GetComponent<T>();

    public static void SendCoroutine(IEnumerator coroutine) => Instance.StartCoroutine(coroutine);

    #endregion
}
