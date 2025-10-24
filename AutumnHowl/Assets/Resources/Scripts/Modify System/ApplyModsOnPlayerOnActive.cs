using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyModsOnPlayerOnActive : MonoBehaviour
{
    [Box, Polymorphic, SerializeReference] protected SerializedModifier_CharacterTargeting modifier;
    Modifier toRemove;
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
        yield return new WaitUntil(() =>
        {
            return GameInstance.Gamestate != null &&
            GameInstance.Gamestate.player != null;
        }
        );

        toRemove = modifier.GetNewRegisteredModifier(new TargetSelf().GetTargetsFrom(GameInstance.Gamestate.player));
    }
}