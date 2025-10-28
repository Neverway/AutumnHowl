using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WB_ModifierIcons : MonoBehaviour
{
    public GameObject modifierIconTemplate;
    public Transform iconContainer;

    [Reload] private static Dictionary<object, Sprite[]> currentIcons;
    [Reload] private static UnityEvent OnIconUpdate;

    public void OnEnable()
    {
        if (currentIcons == null)
        {
            currentIcons = new Dictionary<object, Sprite[]>();
            OnIconUpdate = new UnityEvent();
        }

        OnIconUpdate.AddListener(UpdateIcons);
    }
    public void OnDisable()
    {
        OnIconUpdate.RemoveListener(UpdateIcons);
    }

    public static void AddIcon(object key, Sprite[] icon)
    {
        if(currentIcons.TryAdd(key, icon))
            OnIconUpdate?.Invoke();
    }
    public static void RemoveIcon(object key)
    {
        if(currentIcons.Remove(key))
            OnIconUpdate?.Invoke();
    }

    public void UpdateIcons()
    {
        //Destroy all children
        while (iconContainer.childCount > 0)
            Destroy(iconContainer.GetChild(0).gameObject);

        foreach (Sprite[] sprites in currentIcons.Values)
            foreach (Sprite sprite in sprites)
            {
                GameObject obj = Instantiate(modifierIconTemplate);
                obj.GetComponent<Image>().sprite = sprite;
                obj.transform.SetParent(iconContainer.transform, false);
                obj.SetActive(true);
            }
    }
}
