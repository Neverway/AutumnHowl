using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WB_ModifierIcons : MonoBehaviour
{
    public GameObject modifierIconTemplate;
    public Transform iconContainer;

    public void OnEnable()
    {
        InitializeIfNeeded();
        OnIconUpdate.AddListener(UpdateIcons);
        UpdateIcons();
    }
    public void OnDisable()
    {
        OnIconUpdate.RemoveListener(UpdateIcons);
    }

    public void UpdateIcons()
    {
        //Destroy all children
        for (int i = 0; i < iconContainer.childCount; i++)
            Destroy(iconContainer.GetChild(i).gameObject);

        foreach (Sprite[] sprites in currentIcons.Values)
            foreach (Sprite sprite in sprites)
            {
                GameObject obj = Instantiate(modifierIconTemplate);
                obj.GetComponent<Image>().sprite = sprite;
                obj.transform.SetParent(iconContainer.transform, false);
                obj.SetActive(true);
            }
    }


    //=========== [ Static accessed memebers ] ======================================================================

    [Reload] private static Dictionary<object, Sprite[]> currentIcons;
    [Reload] private static UnityEvent OnIconUpdate;
    private static void InitializeIfNeeded()
    {
        if (currentIcons == null) currentIcons = new Dictionary<object, Sprite[]>();
        if (OnIconUpdate == null) OnIconUpdate = new UnityEvent();
    }
    public static void AddIcon(object key, Sprite[] icon)
    {
        Debug.Log("ICONS ADDEDDDD");
        InitializeIfNeeded();
        if (currentIcons.TryAdd(key, icon))
            OnIconUpdate?.Invoke();
    }
    public static void RemoveIcon(object key)
    {
        Debug.Log("ICONS REMOVEDDD");
        InitializeIfNeeded();
        if (currentIcons.Remove(key))
            OnIconUpdate?.Invoke();
    }


}
