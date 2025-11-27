using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightWard : MonoBehaviour
{
    public float lanternPowerGiven;
    public UnityEvent OnWardShattered;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StealWard()
    {
        GameInstance.Gamestate.currentLanternTime += lanternPowerGiven;
        OnWardShattered.Invoke();
    }
}
