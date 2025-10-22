using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Script governing the behavior of the object spawned when pumpkins die.
/// It flies at the boss and damages the boss.
/// </summary>
public class Bttl_DeadPumpkin : MonoBehaviour
{
    private const float jumpDuration = 1f;
    private const float jumpHeight = 1f;
    [SerializeField] private AnimationCurve heightCurve;
    Char_Battle_PatchyBoss patchyBoss;
    [SerializeField] private AttackElement attack;
    // Start is called before the first frame update
    void Start()
    {
        patchyBoss = FindObjectOfType<Char_Battle_PatchyBoss>();
        if (patchyBoss == null)
        {
            return;
        }
        Vector3 target = patchyBoss.transform.position;
        transform.DOBlendableLocalMoveBy(target - transform.position, jumpDuration);
        transform.DOBlendableLocalMoveBy(Vector3.back * jumpHeight, jumpDuration).SetEase(heightCurve);
        StartCoroutine(DamageBossAfterJump());
    }

    private IEnumerator DamageBossAfterJump()
    {
        yield return new WaitForSeconds(jumpDuration);
        patchyBoss.DoAttack(attack, patchyBoss.gridPawnController.position);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
