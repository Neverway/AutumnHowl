using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Func_WalkNPCTo : MonoBehaviour
{
    public Transform target;
    public float speed;
    public UnityEvent OnWalkEnd = new UnityEvent();

    public void WalkTo(Controller_Overworld_NPC npc) => GameInstance.SendCoroutine(Co_WalkNPC(npc));

    public IEnumerator Co_WalkNPC(Controller_Overworld_NPC npc)
    {
        while (true)
        {
            Vector3 toTarget = target.position - npc.transform.position;
            if (toTarget.magnitude < speed * Time.deltaTime) break;
            toTarget.Normalize();
            npc.movement = toTarget * speed;
            yield return null;
        }
        OnWalkEnd?.Invoke();
    }

}
