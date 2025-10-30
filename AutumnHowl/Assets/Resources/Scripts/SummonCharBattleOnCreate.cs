using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCharBattleOnCreate : MonoBehaviour
{
    public Char_Battle characterToSummon;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => BattleGrid.Instance != null);
        BattleStateController battleController;
        do
        {
            battleController = FindAnyObjectByType<BattleStateController>();
            yield return null;
        } while (battleController == null);

        Vector3 myGridPos = BattleGrid.Instance.transform.InverseTransformPoint(transform.position);
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(myGridPos.x), Mathf.RoundToInt(myGridPos.y));
        battleController.AddCharacter(characterToSummon, position);
    }
}
