using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyModsOnPlayerOnActive : MonoBehaviour
{
    [Box, Polymorphic, SerializeReference] protected SerializedModifier_CharacterTargeting modifier;
    GI_AuHoGameState gameState => GameInstance.Get<GI_AuHoGameState>();
    Modifier toRemove;
    bool onAwake = false;
    public void Awake() => onAwake = true;
    public void OnEnable()
    {
        StartCoroutine(RegisterModifiers());
    }
    public void OnDisable()
    {
        try { toRemove.UnregisterModifier(); } catch { }
    }
    private IEnumerator RegisterModifiers()
    {
        Debug.Log("Hmmm");
        yield return new WaitUntil(() =>
        {

            return gameState != null &&
            gameState.currentGameState != null &&
            gameState.currentGameState.player != null;
        }
        );

        toRemove = modifier.GetNewRegisteredModifier(new TargetSelf().GetTargetsFrom(gameState.currentGameState.player));
    }
}