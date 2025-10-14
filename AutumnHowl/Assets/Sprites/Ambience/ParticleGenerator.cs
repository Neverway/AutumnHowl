#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;



public class ParticleGenerator : MonoBehaviour
{
    public ParticleEffect particles;
    [Space, Header("Spawn Settings")]
    public double spawnsPerSecond = 10f;
    public bool factorSpawnRateByArea = true;
    public double secondsOfSpawningToPreload = 10f;

    public Vector3 area = new Vector3(1f,1f,.1f);
    private double particlesToSpawn;
    private double true_pps => spawnsPerSecond * 
        (factorSpawnRateByArea ? (area.x * area.y * area.z * 0.125f) : 1f );

    public void Start()
    {
        double preloadSpawns = true_pps * secondsOfSpawningToPreload;
        while (preloadSpawns > 0)
        {
            SpawnParticleInArea().SetAnimatorToRandomTime();
            preloadSpawns -= 1f;
        }
    }
    public void Update()
    {
        particlesToSpawn += true_pps * Time.deltaTime;
        while (particlesToSpawn > 0)
        {
            SpawnParticleInArea();
            particlesToSpawn -= 1f;
        }

    }
    public ParticleEffect SpawnParticleInArea()
    {
        ParticleEffect particle = Instantiate(particles, transform);
        particle.transform.position = transform.position;
        particle.transform.position += new Vector3(
            Random.Range(-area.x, area.x) * 0.5f,
            Random.Range(-area.y, area.y) * 0.5f,
            Random.Range(-area.z, area.z) * 0.5f);
        return particle;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ParticleGenerator))]
public class ParticleGeneratorEditor : Editor
{
    private BoxBoundsHandle _boundsHandle = new BoxBoundsHandle();

    private void OnSceneGUI()
    {
        ParticleGenerator generator = (ParticleGenerator)target;

        // The box is always centered on the object
        _boundsHandle.center = Vector3.zero;
        _boundsHandle.size = generator.area;
        Handles.color = Color.yellow;
        Matrix4x4 handleMatrix = generator.transform.localToWorldMatrix;
        using (new Handles.DrawingScope(handleMatrix))
        {
            EditorGUI.BeginChangeCheck();
            _boundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(generator, "Adjust Particle Generator Size");
                generator.area = _boundsHandle.size;
                EditorUtility.SetDirty(generator);
            }
            DrawAxisLines(_boundsHandle.size);
        }
    }
    private void DrawAxisLines(Vector3 size)
    {
        Handles.color = Color.yellow;

        Vector3 half = size * 0.5f;
        Vector3 direction;
        // X axis line
        direction = new Vector3(half.x, 0, 0);
        Handles.DrawLine(direction, -direction);

        // Y axis line
        direction = new Vector3(0, half.y, 0);
        Handles.DrawLine(direction, -direction);

        // Z axis line
        direction = new Vector3(0, 0, half.z);
        Handles.DrawLine(direction, -direction);
    }
}

#endif