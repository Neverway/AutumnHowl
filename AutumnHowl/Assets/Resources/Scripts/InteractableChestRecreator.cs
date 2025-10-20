using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChestRecreator : AutoGUIDObjectRecreator<InteractableChest>, ICreatesGameObject
{
    public GameObject GetCreatedGameObject() => CreateNew().gameObject;
}
