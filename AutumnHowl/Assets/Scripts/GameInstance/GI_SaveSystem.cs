using ErryLib.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GI_SaveSystem : MonoBehaviour
{
    [Polymorphic, SerializeReference] public SaveDataStrategy saveDataStrategy;

    public bool doSaving = true;
    public string saveSlotName = "Slot 0";

    private List<Tuple<SaveAndLoadPropertyAttribute, PropertyInfo>> cachedSaveLoadProperties;
    private List<MethodInfo> cachedInvokeBeforeSaveMethods;
    private List<MethodInfo> cachedInvokeAfterLoadMethods;
    private MethodInfo saveValueMethod;
    private MethodInfo loadValueMethod;
    [Reload] private static GI_SaveSystem instance;

    public void Start()
    {
        if (instance == null)
            instance = this;

        CacheSaveLoadProperties();
        CacheInvokeOnSaveAndLoadMethods();
        CacheSaveLoadValueMethods();
    }
    private void CacheSaveLoadProperties()
    {
        cachedSaveLoadProperties = new List<Tuple<SaveAndLoadPropertyAttribute, PropertyInfo>>();

        foreach (var attribute in ReflectionCache.GetAttributeUsageInfos<SaveAndLoadPropertyAttribute>())
        {
            if (attribute.Member is PropertyInfo property)
            {
                if (!property.IsStatic())
                {
                    Debug.LogError($"{property.DeclaringType}.{property.Name}: You cant put a " +
                        $"SaveAndLoadProperty attribute on a non-static property");
                    continue;
                }
                if (!property.CanRead || !property.CanWrite)
                {
                    Debug.LogError($"{property.DeclaringType}.{property.Name}: The property with the " +
                        $"SaveAndLoadProperty attribute must have a Set AND a Get method");
                    continue;
                }
                var castedAttribute = attribute.As<SaveAndLoadPropertyAttribute>();
                var toCache = new Tuple<SaveAndLoadPropertyAttribute, PropertyInfo>(castedAttribute, property);
                cachedSaveLoadProperties.Add(toCache);
            }
            else
                Debug.LogError("You cant put a SaveAndLoadProperty attribute on a non-property");
        }
    }
    private void CacheInvokeOnSaveAndLoadMethods()
    {
        cachedInvokeBeforeSaveMethods = new List<MethodInfo>();

        foreach (var attribute in ReflectionCache.GetAttributeUsageInfos<InvokeBeforeSaveAttribute>())
        {
            if (attribute.Member is MethodInfo method)
            {
                if (!method.IsStatic())
                {
                    Debug.LogError($"{method.DeclaringType}.{method.Name}: You cant put an " +
                        $"InvokeBeforeSave attribute on a non-static method");
                    continue;
                }
                if (method.ContainsGenericParameters || !method.HasParametersNone())
                {
                    Debug.LogError($"{method.DeclaringType}.{method.Name}: This method with the " +
                        $"InvokeBeforeSave attribute must have no parameters of any kind");
                    continue;
                }
                cachedInvokeBeforeSaveMethods.Add(method);
            }
            else
                Debug.LogError("You cant put an InvokeBeforeSave attribute on a non-property");
        }

        cachedInvokeAfterLoadMethods = new List<MethodInfo>();

        foreach (var attribute in ReflectionCache.GetAttributeUsageInfos<InvokeAfterLoadAttribute>())
        {
            if (attribute.Member is MethodInfo method)
            {
                if (!method.IsStatic())
                {
                    Debug.LogError($"{method.DeclaringType}.{method.Name}: You cant put an " +
                        $"InvokeAfterLoad attribute on a non-static method");
                    continue;
                }
                if (method.ContainsGenericParameters || !method.HasParametersNone())
                {
                    Debug.LogError($"{method.DeclaringType}.{method.Name}: This method with the " +
                        $"InvokeAfterLoad attribute must have no parameters of any kind");
                    continue;
                }
                cachedInvokeAfterLoadMethods.Add(method);
            }
            else
                Debug.LogError("You cant put an InvokeAfterLoad attribute on a non-property");
        }
    }
    private void CacheSaveLoadValueMethods()
    {
        saveValueMethod = typeof(GI_SaveSystem)
            .GetMethod(nameof(SaveValue), BindingFlags.Static | BindingFlags.Public);
        loadValueMethod = typeof(GI_SaveSystem)
            .GetMethod(nameof(LoadValue), BindingFlags.Static | BindingFlags.Public);
    }
    [ContextMenu("Trigger Save Game")]
    private void OnSaveGame()
    {
        if (!Application.isPlaying || !doSaving) return;
        Debug.Log($"Saving Game to slot \"{saveSlotName}\"");

        //Call all methods with InvokeBeforeSave attributes
        foreach (var beforeSaveMethod in cachedInvokeBeforeSaveMethods)
            beforeSaveMethod.Invoke(null, null);

        //Save all values from properties with SaveAndLoadProperty attributes
        foreach (var saveLoadProperty in cachedSaveLoadProperties)
        {
            var value = saveLoadProperty.Item2.GetValue(null);
            var saveMethod = saveValueMethod.MakeGenericMethod(saveLoadProperty.Item2.PropertyType);
            saveMethod.Invoke(this, new object[] { value, saveLoadProperty.Item1.saveId });
        }
    }
    [ContextMenu("Trigger Load Game")]
    private void OnLoadGame()
    {
        if (!Application.isPlaying || !doSaving) return;
        Debug.Log($"Loading Game from slot \"{saveSlotName}\"");

        //Load all values to properties with SaveAndLoadProperty attributes
        foreach (var saveLoadProperty in cachedSaveLoadProperties)
        {
            var value = saveLoadProperty.Item2.GetValue(null);
            var loadMethod = loadValueMethod.MakeGenericMethod(saveLoadProperty.Item2.PropertyType);
            var loadedValue = loadMethod.Invoke(this, new object[] { value, saveLoadProperty.Item1.saveId });
            saveLoadProperty.Item2.SetValue(null, loadedValue);
        }

        //Call all methods with InvokeAfterLoad attributes
        foreach (var afterLoadMethod in cachedInvokeAfterLoadMethods)
            afterLoadMethod.Invoke(null, null);
    }

    public static void SaveGame() => instance.OnSaveGame();
    public static void LoadGame() => instance.OnLoadGame();

    public static void SaveValue<T>(T value, string id) => instance.saveDataStrategy.SaveValue(value, ToSaveSlotID(id));
    public static T LoadValue<T>(T defaultValue, string id) => instance.saveDataStrategy.LoadValue(defaultValue, ToSaveSlotID(id));
    private static string ToSaveSlotID(string id) => $"Game_{instance.saveSlotName}_{id}";
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SaveAndLoadPropertyAttribute : Attribute 
{
    public string saveId;
    public SaveAndLoadPropertyAttribute([CallerMemberName] string saveId = "")
    {
        this.saveId = saveId;
    }
}
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class InvokeBeforeSaveAttribute : Attribute { public InvokeBeforeSaveAttribute() { } }


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class InvokeAfterLoadAttribute : Attribute { public InvokeAfterLoadAttribute() { } }