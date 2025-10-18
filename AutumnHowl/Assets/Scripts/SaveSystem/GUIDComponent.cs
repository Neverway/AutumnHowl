using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GUIDComponent : MonoBehaviour
{
    [StringAsGUID, SerializeField] private string guid;
    public string GetGUID() => guid;
    public string SetGUID(string newGuid) => guid = newGuid;

    public string NewGUID()
    {
        guid = Guid.NewGuid().ToString();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        return guid;
    }

    private void Reset() => NewGUID();
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Event.current != null)
        {
            if (Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste")
                    NewGUID();
            }
            if (Event.current.type == EventType.DragUpdated)
            {
                NewGUID();
            }

        }
#endif
    }

}
 /*
#if UNITY_EDITOR
[InitializeOnLoad]
public static class StringAsGUIDInitializer
{
   static StringAsGUIDInitializer()
   {
       ObjectFactory.componentWasAdded += OnComponentAdded;
   }

   private static void OnComponentAdded(Component component)
   {
        if (component is GUIDComponent guidComponent)
            guidComponent.NewGUID();
   }
}
#endif
// */