using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSAVE_BoulderBox : AutoGUIDObjectRecreator<TESTSAVE_Boulder>
{
    public Transform targetBoulderLocation;
    public void CreateBoulder()
    {
        var boulder = CreateNew();
        boulder.transform.position = targetBoulderLocation.position;
    }
}
