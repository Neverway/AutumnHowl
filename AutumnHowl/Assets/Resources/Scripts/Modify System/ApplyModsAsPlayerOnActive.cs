using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyModsAsPlayerOnActive : MonoBehaviour
{
    [Box, Polymorphic, SerializeReference] protected EffectActionTarget targets = new TargetSelf();
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
        if (targets == null) targets = new TargetSelf();
        toRemove = modifier.GetNewRegisteredModifier(targets.GetTargetsFrom(GameInstance.Gamestate.player));
    }
}