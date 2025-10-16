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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used in the battle widget to give functionality to the hit compass
/// </summary>
public class BattleAttackCompass : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Tooltip("The size of the angle that registers as a good hit")]
    [SerializeField] private float goodAngle = 30f;
    [Tooltip("The size of the angle that registers as a perfect hit (this should be smaller than the good angle)")]
    [SerializeField] private float perfectAngle = 10f;
    [Tooltip("?")]
    [SerializeField] public bool stopByTapping = false;
    [Tooltip("The duration for the hit text to be visible")]
    [SerializeField] private float hitTextDuration = 0.75f;
    [Tooltip("How fast the sword needle travels around the compass")]
    [SerializeField] private float spinSpeed = 130f;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    [Tooltip("Used to keep track of when teh attack bar started")]
    public bool hasInitialized;
    [Tooltip("Used to track when the attack bar is in progress")]
    private bool attackBarActive;
    [Tooltip("The current angle the sword needle is pointing in")]
    private float swordAngle = 0f;

    private bool bufferLeft = false;
    private bool bufferRight = false;
    
    public static float north { get; private set; } = 0;
    public static float east { get; private set; } = 90;
    public static float south { get; private set; } = 180;
    public static float west { get; private set; } = 270;
    public enum cardinalDirection { north, south, west, east }
    private enum RingState { notStarted, spinning, finish }
    private RingState currentState = RingState.notStarted;
    private enum SpinDirection { left, right }
    private SpinDirection currentSpinDirection;
    //Tracks the amount the compass has spun (positive or negative) to determine what way to swing the sword.
    private float totalSpin = 0f;
    [SerializeField] public Image centerFill;
    
    
    private float nearestAngleToSword;
    private float distanceFromNearestAngle;

    /// <summary>
    /// The eight spaces around a tile in clockwise order.
    /// Used for attack patterns.
    /// </summary>
    private Vector2Int[] swingPattern =
    {
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,1),
    };

    private int spinStartIndex = 0;

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [Tooltip ("Reference to the player so we can freeze them when attacking")]
    private Char_Battle_Player player;
    [Tooltip("Keep track of the active hit text coroutine so we make sure only one is running")]
    private Coroutine showHitTextCoroutine;
    [Tooltip("The 4 images that are used to fill the 4 bars for this hit angle")]
    [SerializeField] private Image[] goodBarImages, perfectBarImages;
    [Tooltip("Text used to display how good the hit angle was")]
    [SerializeField] private TMP_Text hitText;
    [Tooltip("The image that represents the sword angle on the attack compass")]
    [SerializeField]private Image needleImage;
    //The object used for generated sword swing attacks.
    [SerializeField] private GameObject defaultAttackObject;
    private Coroutine resetRoutine;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public void Start()
    {
        player = FindObjectOfType<Char_Battle_Player>();
    }

    public void Update()
    {
        // Update the needle based on the sword angle
        needleImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0f, -swordAngle));
        
        // Detect activation
        if (!attackBarActive)
        {
            SetNeedleDirection(player.movement);
            spinStartIndex = (int)swordAngle / 90;
            // Start the attack timer on first press
            if (GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                currentSpinDirection = SpinDirection.left;
                Initialize();
            }
            else if (GameInstance.Inputs.Action.WasPressedThisFrame())
            {
                currentSpinDirection = SpinDirection.right;
                Initialize();
            }
            return;
        }
        
        // Get inputs when active
        if (currentState == RingState.spinning)
        {
            DoSpinState();
        }
    }

    public void OnEnable()
    {
        Reset();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    /// <summary>
    /// Freeze the player movement and enable the attack bar
    /// </summary>
    private void Initialize()
    {
        if (hasInitialized) return;
        currentState = RingState.spinning;
        
        centerFill.gameObject.transform.localRotation = Quaternion.Euler (0, 0, -swordAngle);
        centerFill.fillAmount = 0f;
        totalSpin = 0f;
        
        player.canMove = false;
        attackBarActive = true;
        hasInitialized = true;
    }

    /// <summary>
    /// Coroutine to delay the reset of the compass after an attack, to add cooldown before the player can attack again
    /// </summary>
    private IEnumerator CoReset()
    {
        currentState = RingState.finish;
        swordAngle = nearestAngleToSword;
        //yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0f);
        Reset();
        resetRoutine = null;
    }

    /// <summary>
    /// Resets the attack compass so another attack can be performed
    /// </summary>
    private void Reset()
    {
        SetupRingColors();
        
        currentState = RingState.notStarted;
        attackBarActive = false;
        hasInitialized = false;
    }
    
    /// <summary>
    /// Resets the minigame.     
    /// </summary>
    private void OnAttackDone()
    {
        var attack = 0;
        bool mirrorX = false;
        bool mirrorY = false;

        if (resetRoutine != null)
        {
            StopCoroutine (resetRoutine);
        }
        resetRoutine = StartCoroutine(CoReset());
    }
    
    /// <summary>
    /// Adjust the bar images to match the defined hit angles
    /// </summary>
    private void SetupRingColors ()
    {
        for (int i = 0; i < 4; i++)
        {
            goodBarImages[i].gameObject.transform.localRotation = Quaternion.Euler (0, 0, goodAngle + (90 * i));
            goodBarImages[i].fillAmount = ((goodAngle * 2f)) / 360;
            perfectBarImages[i].gameObject.transform.localRotation = Quaternion.Euler (0, 0, perfectAngle + (90 * i));
            perfectBarImages[i].fillAmount = ((perfectAngle * 2f)) / 360;
        }
    }
    
    private void SetNeedleDirection(float _direction)
    {
        swordAngle = _direction;
    }
    
    private void SetNeedleDirection(Vector2 _movement)
    {
        switch (_movement.x , _movement.y)
        {
            case (0, 1):
                SetNeedleDirection(south);
                break;
            case (0, -1):
                SetNeedleDirection(north);
                break;
            case (-1, 0):
                SetNeedleDirection(east);
                break;
            case (1, 0):
                SetNeedleDirection(west);
                break;
        }
    }
    
    private void DoSpinState ()
    {/*
        if (stopByTapping == false)
        {
            //buffer the next spin if player presses the opposite direction input during the spin
            if (currentSpinDirection == SpinDirection.left && GameInstance.Inputs.Action.WasPressedThisFrame())
            {
                bufferRight = true;
            }
            if (currentSpinDirection == SpinDirection.right && GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                bufferLeft = true;
            }
            //cancel the buffered input if the player releases the direction input
            if (bufferLeft && GameInstance.Inputs.Interact.WasPressedThisFrame() == false)
            {
                bufferLeft = false;
            }
            if (bufferRight && GameInstance.Inputs.Action.WasPressedThisFrame()== false)
            {
                bufferRight = false;
            }
            //End the spin upon key released
            if (currentSpinDirection == SpinDirection.left && GameInstance.Inputs.Interact.WasPressedThisFrame() == false)
            {
                FinishSpin ();
                return;
            }
            if (currentSpinDirection == SpinDirection.right && GameInstance.Inputs.Action.WasPressedThisFrame()== false)
            {
                FinishSpin ();
                return;
            }
        }*/
        if (stopByTapping == true)
        {
            if (GameInstance.Inputs.Action.WasPressedThisFrame() || GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                FinishSpin ();
                return;
            }
        }
        float spinAmount = 0f;
        if (currentSpinDirection == SpinDirection.left)
        {
            spinAmount = -spinSpeed * Time.deltaTime;
        }
        if (currentSpinDirection == SpinDirection.right)
        {
            spinAmount = spinSpeed * Time.deltaTime;
        }
        swordAngle += spinAmount;
        totalSpin += spinAmount;
        //Clamps the totalSpin, but only if it goes far enough past 360 that we've looped around to a 90-degrees swing again.
        //The cutoff is 45 degrees past 360, since that would clamp to 90 degrees.
        if (totalSpin > 360 + 45)
        {
            totalSpin -= 360;
        }
        if (totalSpin < -360 - 45)
        {
            totalSpin += 360;
        }
        while (swordAngle > 360f)
        {
            swordAngle -= 360f;
        }
        while (swordAngle < 0f)
        {
            swordAngle += 360f;
        }
        PlaceCenterFill ();
    }

    private void PlaceCenterFill ()
    {
        if (totalSpin > 0)
        {
            centerFill.fillAmount = totalSpin / 360f;
            return;
        }
        if (totalSpin < 0f)
        {
            centerFill.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0f, -swordAngle));
            centerFill.fillAmount = -totalSpin / 360;
        }
    }


    /// <summary>
    /// Ends sword spinning and calculates the direction it was pointing.
    /// </summary>
    private void FinishSpin ()
    {
        if (Mathf.Abs(totalSpin) <= 45)
        {
            FailAttack ();
            return;
        }
        nearestAngleToSword = north;
        float test = Mathf.Abs (Mathf.DeltaAngle (swordAngle, north));
        distanceFromNearestAngle = test;
        test = Mathf.Abs (Mathf.DeltaAngle (swordAngle, east));
        if (test < distanceFromNearestAngle)
        {
            distanceFromNearestAngle = test;
            nearestAngleToSword = east;
        }
        test = Mathf.Abs (Mathf.DeltaAngle (swordAngle, south));
        if (test < distanceFromNearestAngle)
        {
            distanceFromNearestAngle = test;
            nearestAngleToSword = south;
        }
        test = Mathf.Abs(Mathf.DeltaAngle (swordAngle, west));
        if (test < distanceFromNearestAngle)
        {
            distanceFromNearestAngle = test;
            nearestAngleToSword = west;
        }

        if (distanceFromNearestAngle < perfectAngle)
        {
            ShowHitText ("Perfect!");
        }
        else if (distanceFromNearestAngle < goodAngle) {
            ShowHitText ("Good");
        }
        else
        {
            ShowHitText ("Miss!");
        }
        ClampTotalSpin ();
        print("Clamped spin:" + totalSpin);
        
        ExecuteAttack();
        centerFill.fillAmount = 0;
    }    

    private void ClampTotalSpin ()
    {
        totalSpin = Mathf.RoundToInt (totalSpin / 90f);
    }
    
    private void ShowHitText(string _text)
    {
        if (showHitTextCoroutine != null)
        {
            StopCoroutine (showHitTextCoroutine);
        }
        showHitTextCoroutine = StartCoroutine (CoShowHitText (_text));
    }

    private IEnumerator CoShowHitText(string _text)
    {
        hitText.SetText (_text);
        yield return new WaitForSeconds (hitTextDuration);
        hitText.SetText ("");
    }


    private void ExecuteAttack()
    {
        var sequence = new AttackSequence ();
        sequence.attacks = new List<AttackElement> ();
        int n = spinStartIndex * 2;
        int increment = MathF.Sign (totalSpin);
        //Generate an attack by looping through the swingPattern
        for (int i = 0; i < Mathf.Abs(totalSpin*2)+1; i++)
        {
            AttackElement attack = new AttackElement ();
            attack.position = swingPattern[n];
            attack.visualEffect = defaultAttackObject;
            sequence.attacks.Add (attack);
            n += increment;
            if (n < 0)
            {
                n += swingPattern.Length;
            }
            n = n % swingPattern.Length;
        }
        //Decrement n by 1 so we can set player movement correctly.
        n -= increment;
        if (n < 0)
        {
            n += swingPattern.Length;
        }
        n = n % swingPattern.Length;
        GenerateAttackDirections (sequence);
        player.AttackSequences[0] = sequence;
        player.PerformGeneratedAttack();
        OnAttackDone();
        //player.movement = swingPattern[n] * -1;
        print (player.movement);
        if (Mathf.Abs(totalSpin) > 2)
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.slash3, 1);
        }
        else if (Mathf.Abs (totalSpin) > 1)
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.slash2, 1);
        }
        else if (Mathf.Abs (totalSpin) > 0)
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.slash1, 1);
        }
        else
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.failBuzz, 1);
        }
    }

    private void FailAttack ()
    {
        ShowHitText ("Miss!");
        AudioManager.Instance.PlayClip (AudioManager.Instance.failBuzz);
        centerFill.fillAmount = 0;
        OnAttackDone ();
        player.SkipTurn ();
    }

    /// <summary>
    /// Generates attack directions for an AttackSequence based on the position of attacks.
    /// </summary>
    /// <param name="sequence"></param>
    private void GenerateAttackDirections (AttackSequence sequence)
    {
        if (sequence.attacks.Count == 0)
        {
            return;
        }
        if (sequence.attacks.Count == 1)
        {
            sequence.attacks[0].direction = sequence.attacks[0].position;
        }
        Vector2Int dir;
        for (int i = 0; i < sequence.attacks.Count; i+=2)
        {
            if (i + 1 == sequence.attacks.Count && i > 0)
            {
                sequence.attacks[i].direction = (sequence.attacks[i].position - sequence.attacks[i-1].position);
                break;
            }

            dir = sequence.attacks[i].position - sequence.attacks[i + 1].position;
            sequence.attacks[i].direction = -dir;
            sequence.attacks[i+1].direction = -dir;
            sequence.attacks[i + 1].direction = -dir;
            print ("direction for " + i + " = " + -dir);
        }
    }

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
