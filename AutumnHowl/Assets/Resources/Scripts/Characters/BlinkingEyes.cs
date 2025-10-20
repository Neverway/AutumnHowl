using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingEyes : MonoBehaviour
{

    [SerializeField] private float onTime = 7f;
    [SerializeField] private float offTime = 0.5f;
    private float timer = 0f;
    private float maxTime;
    [SerializeField] private Renderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        maxTime = onTime + offTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > onTime)
        {
            sprite.enabled = false;
        }
        else
        {
            sprite.enabled = true;
        }

        if (timer > maxTime)
        {
            timer -= maxTime;
        }
    }
}
