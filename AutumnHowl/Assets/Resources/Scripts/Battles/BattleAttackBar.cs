//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleAttackBar : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public float resistance;
    public float push;
    public float enduranceTime;
    public float enduranceExpension;
    public Vector2 attack1Range, attack2Range, attack3Range;
    public int attackLeft, attackRight;
    public bool attackBarActive;
    public bool hasInitialized;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public float currentProgressLeft, currentProgressRight, currentEnduranceTime;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Image frame, leftBar, rightBar, leftTimer, rightTimer;
    public Color colorDefault, colorAttack1, colorAttack2, colorAttack3, colorAttack4, colorFailed;
    public Char_Battle_Player player;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public void Start()
    {
        player = FindObjectOfType<Char_Battle_Player>();
    }

    public void Update()
    {
        if (!attackBarActive)
        {
            // Start the attack timer on first press
            if (GameInstance.Inputs.Interact.WasPressedThisFrame() || GameInstance.Inputs.Action.WasPressedThisFrame()) Initialize();
            
            return;
        }
        
        if (GameInstance.Inputs.Interact.WasPressedThisFrame())
        {
            if (currentProgressLeft < 1)
            {
                currentProgressLeft += push;
            }
        }
        if (GameInstance.Inputs.Action.WasPressedThisFrame())
        {
            if (currentProgressRight < 1)
            {
                currentProgressRight += push;
            }
        }
    }

    public void FixedUpdate()
    {
        if (!attackBarActive) return;
        UpdateLeftBar();
        UpdateRightBar();
        UpdateTimer();
    }

    public void Initialize()
    {
        if (hasInitialized) return;
        player.canMove = false;
        attackBarActive = true;
        hasInitialized = true;
    }

    public IEnumerator CoReset()
    {
        yield return new WaitForSeconds(0.5f);
        Reset();
    }

    public void Reset()
    {
        frame.color = Color.white;
        leftBar.color = colorDefault;
        rightBar.color = colorDefault;
        leftBar.fillAmount = 0;
        rightBar.fillAmount = 0;
        leftTimer.fillAmount = 1;
        rightTimer.fillAmount = 1;
        currentProgressLeft = 0;
        currentProgressRight = 0;
        currentEnduranceTime = enduranceTime;
        attackBarActive = false;
        hasInitialized = false;
    }

    public void OnEnable()
    {
        Reset();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateLeftBar()
    {
        // Decrement progress
        if (currentProgressLeft > 0)
        {
            currentProgressLeft -= resistance;
        }

        // Target 1 reached
        if (currentProgressLeft >= attack1Range.x && currentProgressLeft <= attack1Range.y)
        {
            leftBar.color = colorAttack1;
            attackLeft = 1;
        }
        // Target 2 reached
        else if (currentProgressLeft >= attack2Range.x && currentProgressLeft <= attack2Range.y)
        {
            leftBar.color = colorAttack2;
            attackLeft = 2;
        }
        // Target 4 reached
        else if (currentProgressLeft >= attack3Range.x && currentProgressLeft <= attack3Range.y && attackRight >= 3)
        {
            leftBar.color = colorAttack4;
            attackLeft = 4;
        }
        // Target 3 reached
        else if (currentProgressLeft >= attack3Range.x && currentProgressLeft <= attack3Range.y)
        {
            leftBar.color = colorAttack3;
            attackLeft = 3;
        }
        else
        {
            leftBar.color = colorDefault;
            attackLeft = 0;
        }
        
        // Update bar appearance
        currentProgressLeft = Mathf.Clamp(currentProgressLeft, 0, 1);
        leftBar.fillAmount = currentProgressLeft;
    }
    
    private void UpdateRightBar()
    {
        // Decrement progress
        if (currentProgressRight > 0)
        {
            currentProgressRight -= resistance;
        }

        // Target 1 reached
        if (currentProgressRight >= attack1Range.x && currentProgressRight <= attack1Range.y)
        {
            rightBar.color = colorAttack1;
            attackRight = 1;
        }
        // Target 2 reached
        else if (currentProgressRight >= attack2Range.x && currentProgressRight <= attack2Range.y)
        {
            rightBar.color = colorAttack2;
            attackRight = 2;
        }
        // Target 4 reached
        else if (currentProgressRight >= attack3Range.x && currentProgressRight <= attack3Range.y && attackLeft >= 3)
        {
            rightBar.color = colorAttack4;
            attackRight = 4;
        }
        // Target 3 reached
        else if (currentProgressRight >= attack3Range.x && currentProgressRight <= attack3Range.y)
        {
            rightBar.color = colorAttack3;
            attackRight = 3;
        }
        else
        {
            rightBar.color = colorDefault;
            attackRight = 0;
        }
        
        // Update bar appearance
        currentProgressRight = Mathf.Clamp(currentProgressRight, 0, 1);
        rightBar.fillAmount = currentProgressRight;
    }

    private void UpdateTimer()
    {
        if (currentEnduranceTime > 0)
        {
            currentEnduranceTime -= enduranceExpension;
        }
        else
        {
            OnAttackDone();
            attackBarActive = false;
        }

        float normalizedTime = currentEnduranceTime / enduranceTime;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        leftTimer.fillAmount = normalizedTime;
        rightTimer.fillAmount = normalizedTime;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void OnAttackDone()
    {
        var attack = 0;
        bool mirrorX = false;
        bool mirrorY = false;

        if (attackLeft > attackRight)
        {
            attack = attackLeft;
        }
        else if (attackLeft < attackRight)
        {
            attack = attackRight;
            mirrorX = true;
        }
        else
        {
            attack = attackLeft;
        }
        
        switch (attack)
        {
            case 0:
                frame.color = colorFailed;
                player.battleStateController.NextTurnStep();
                break;
            case 1:
                frame.color = colorAttack1;
                player.PerformAttack(0, mirrorX, mirrorY);
                break;
            case 2:
                frame.color = colorAttack2;
                player.PerformAttack(1, mirrorX, mirrorY);
                break;
            case 3:
                frame.color = colorAttack3;
                player.PerformAttack(2, mirrorX, mirrorY);
                break;
            case 4:
                frame.color = colorAttack4;
                player.PerformAttack(3, mirrorX, mirrorY);
                break;
        }

        StartCoroutine(CoReset());
    }


    #endregion
}
