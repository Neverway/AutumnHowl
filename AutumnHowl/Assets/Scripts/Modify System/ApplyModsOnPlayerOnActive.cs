using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyModsOnPlayerOnActive : MonoBehaviour
{
    [Box, Polymorphic, SerializeReference] protected ICharacterStatModInstancer modifier;
    GI_AuHoGameState gameState;
    Modifier toRemove;
    bool onAwake = false;
    public void Awake() => onAwake = true;
    public void OnEnable()
    {
        StartCoroutine(RegisterModifiers());
    }
    public void OnDisable()
    {
        toRemove.UnregisterModifier();
    }
    private IEnumerator RegisterModifiers()
    {
        if (onAwake)
        {
            yield return null;
            yield return null;
        }

        gameState = GameInstance.Get<GI_AuHoGameState>();
        toRemove = modifier.GetNewRegisteredModifier(
            new TargetSelf().GetTargetsFrom(gameState.currentGameState.player));
    }
}