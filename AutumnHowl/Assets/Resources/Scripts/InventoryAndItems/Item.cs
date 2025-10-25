//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using ErryLib.Reflection;
using UnityEngine;

public abstract class Item : ScriptableObject, UniquelyIdentifiable
{
    /*-----[ UniqueID Setup ]-----------------------------------------------------------------------------------------*/
    [field: SerializeField] public string UniqueID { get; protected set; }
    [InvokeOnReflectionCacheLoadRuntime] public static void CacheIDs() => IDToObj<Item>.AddAllFromUnityResources();


    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public string displayName;
    [TextArea, SerializeField] protected string description;
    public bool canNotDiscard;
    public bool allowMultiple = true;
    public int buyCost;
    public int sellCost;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual string GetDescription() => description;
    public bool TryUse(CharacterIdentifier user, int _atIndex, int _inList = 0)
    {
        if (new Event_UseItem(this, user).InvokeAndGetIfSuccess())
            return OnUse(user, _atIndex, _inList);

        return false;
    }
    protected abstract bool OnUse(CharacterIdentifier user, int _atIndex, int _inList = 0);


    #endregion
}
