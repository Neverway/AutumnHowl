using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Saves only the IMMEDIATE child active states of this object</summary>
public class Utility_SaveActiveStatesOfObjects : AutoGUIDObject<Wrapper<bool[]>>
{
    //[SerializeField] private Transform[] transforms;
    //public override Wrapper<bool[]> OnSaveInstance() => 
    //    new Wrapper<bool[]>(transforms.Select(t => t.gameObject.activeSelf).ToArray());
    //public override void OnLoadInstance(Wrapper<bool[]> data)
    //{
    //    if (data == null) return;
    //    for(int i  = 0; i < transforms.Length; ++i)
    //        transforms[i].gameObject.SetActive(false);
    //
    //}
    //public override void OnNewInstance() { }
    public override void OnLoadInstance(global::Wrapper<bool[]> data)
    {
        throw new System.NotImplementedException();
    }

    public override void OnNewInstance()
    {
        throw new System.NotImplementedException();
    }

    public override global::Wrapper<bool[]> OnSaveInstance()
    {
        throw new System.NotImplementedException();
    }
}
