using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnUnveil : AutoGUIDObject<bool>
{
    public Transform toEnable;
    public GameObject unveilEffect;

    public void Awake()
    {
        toEnable.gameObject.SetActive(false);
    }

    public bool Unveil()
    {
        //Unveil effect
        if (unveilEffect != null)
            Instantiate(unveilEffect).transform.position = toEnable.position;

        if (toEnable.gameObject.activeSelf) return false;
        toEnable.gameObject.SetActive(true);
        return true;
    }

    public override void OnLoadInstance(bool data) => toEnable.gameObject.SetActive(data);
    public override void OnNewInstance() => OnLoadInstance(false);
    public override bool OnSaveInstance() => toEnable.gameObject.activeSelf;
}

[Serializable]
public class UnveilAction : EffectAction
{
    public float unveilDistance = 5f;
    public override void ApplyEffect(CharacterIdentifier user)
    {
        Debug.Log("Unveiling!");
        EnableOnUnveil[] toUnveil = GameObject.FindObjectsByType<EnableOnUnveil>(FindObjectsSortMode.None);
        bool unveilSuccess = false;
        foreach (EnableOnUnveil obj in toUnveil)
        {
            if (Vector3.Distance(obj.toEnable.transform.position, GameInstance.Playerbody.transform.position) <= unveilDistance)
                unveilSuccess |= obj.Unveil();
        }
    }

    public override string DescribeNoFormat() => "Reveals Hidden Objects";
}
