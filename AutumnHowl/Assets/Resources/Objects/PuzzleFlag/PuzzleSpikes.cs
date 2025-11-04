using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpikes : MonoBehaviour
{
    bool doingGameOver;
    public void Update()
    {
        if (doingGameOver)
            return;

        if (Vector3.Distance(transform.position, GameInstance.Playerbody.transform.position) <= 0.5f)
        {
            doingGameOver = true;
            GameInstance.SendCoroutine(Co_GameOver());
        }
    }

    public IEnumerator Co_GameOver()
    {
        GameInstance.Playerbody.Stats.ModifyHealth(-9999);
        yield return new WaitForSeconds(0.5f);
        GameInstance.Get<GI_WorldLoader>().Load("GameOver");
    }
}
