using UnityEngine;

public class TESTSAVE_Boulder : AutoGUIDObject<Vector3>
{
    [field: SerializeField] public GameObject GUIDObjectPrefab { get; set; }


    public override Vector3 OnSaveInstance() => transform.position;
    public override void OnLoadInstance(Vector3 data) => transform.position = data;
    public override void OnNewInstance() { }
}
