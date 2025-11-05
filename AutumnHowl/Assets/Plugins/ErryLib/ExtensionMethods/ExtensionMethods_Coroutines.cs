using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods_Coroutines
{
    public static IEnumerator UsingSeed(this IEnumerator enumerator, int seed)
    {
        var oldSeedState = Random.state;

        Random.InitState(seed);
        var enumeratorSeedState = Random.state;

        Random.state = oldSeedState;

        while (true)
        {
            oldSeedState = Random.state;

            Random.state = enumeratorSeedState;
            if (!enumerator.MoveNext())
            {
                //Debug.Log("COROUTINE END");
                Random.state = oldSeedState;
                yield break;
            }
            //Debug.Log("COROUTINE CONTINUE");
            yield return enumerator.Current;
            enumeratorSeedState = Random.state;

            Random.state = oldSeedState;
        }
    }
}
