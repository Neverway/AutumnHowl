using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TEST_SAVING : MonoBehaviour
{
    public void Start()
    {
        myGuyyy = this;
    }

    //----------Saving and Loading-------------------------------------------------------
    public static TEST_SAVING myGuyyy;
    [SaveAndLoadProperty] public static Vector3 SavedPosition { get; set; }
    [InvokeBeforeSave] public static void BeforeSave() => Debug.Log(SavedPosition = myGuyyy.transform.position);
    [InvokeAfterLoad] public static void AfterLoad() => Debug.Log(myGuyyy.transform.position = SavedPosition);
}
