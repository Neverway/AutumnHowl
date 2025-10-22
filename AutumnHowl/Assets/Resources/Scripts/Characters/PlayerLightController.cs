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
using DG.Tweening;
using UnityEngine;

public class PlayerLightController : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public Color normalColor, extinguishedColor, extinguishedColor2;
    public float normalRange, extinguishedRange;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Light[] lights;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void SetLanternLightState(bool _extinguished)
    {
        if (_extinguished)
        {
            foreach (var _light in lights)
            {
                if (_light.name != "Light (Far)") _light.DOColor(extinguishedColor, 1);
                else _light.DOColor(extinguishedColor2, 1);
                // This just lerps between the normal light range and the extinguished range
                // => is essentially the shorthand for creating a function and returning a value all in one go
                // (I think) ~Liz
                DOVirtual.Float(normalRange, extinguishedRange, 1, (newValue) => { _light.range = newValue; });
            }
        }
        else
        {
            foreach (var _light in lights)
            {
                _light.DOColor(normalColor, 1);
                DOVirtual.Float(extinguishedRange, normalRange, 1, (newValue) => { _light.range = newValue; });
            }
        }
    }


    #endregion
}
