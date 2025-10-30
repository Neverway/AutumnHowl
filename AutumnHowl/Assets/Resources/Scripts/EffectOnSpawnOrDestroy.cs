using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOnSpawnOrDestroy : MonoBehaviour
{
    public bool onDestroy;
    public bool onSpawn;

    [Box, Polymorphic, SerializeReference] public EffectAction effectAction;

    public void Start()
    {
        if (onSpawn)
            effectAction.ApplyEffect(GameInstance.Playerbody.Identifier);
    }
    public void OnDestroy()
    {
        if (onDestroy)
            effectAction.ApplyEffect(GameInstance.Playerbody.Identifier);
    }
}
