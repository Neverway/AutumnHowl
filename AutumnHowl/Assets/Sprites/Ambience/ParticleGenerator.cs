using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGenerator : MonoBehaviour
{
    public ParticleEffect particles;
    public Vector2 spawnArea;
    public double particlesPerSecond;

    private double particlesToSpawn;

    public void Update()
    {
        particlesToSpawn += particlesPerSecond * Time.deltaTime;
        while (particlesToSpawn > 0)
        {
            ParticleEffect particle = Instantiate(particles, transform);
            particle.transform.position = transform.position;
            particle.transform.position += new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                transform.position.z);
            particlesToSpawn -= 1f;
        }

    }

    public void OnDrawGizmosSelected()
    {
        Vector3 size = new Vector3(spawnArea.x * 2f, spawnArea.y * 2f);
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawCube(transform.position, size);
        Gizmos.color = new Color(1f, 1f, 0f, 1f);
        Gizmos.DrawWireCube(transform.position, size);
    }
    public void OnDrawGizmos()
    {
        Vector3 size = new Vector3(spawnArea.x * 2f, spawnArea.y * 2f);
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
