using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class ApplyModsOnPlayerOnActive : MonoBehaviour
{
    [Box, Polymorphic, SerializeReference] protected ICharacterStatModInstancer modifier;
    GI_AuHoGameState gameState;
    Modifier toRemove;
    public void OnEnable()
    {

        gameState = GameInstance.Get<GI_AuHoGameState>();
        toRemove = modifier.GetNewRegisteredModifier(
            new TargetSelf().GetTargetsFrom(gameState.currentGameState.player));
    }
    public void OnDisable()
    {
        toRemove.UnregisterModifier();
    }
}