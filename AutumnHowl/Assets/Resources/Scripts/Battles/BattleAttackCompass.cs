//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using DG.Tweening;
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
    [Tooltip("When true, you have to press and press again to trigger attack. When false, you press and hold and trigger attack on release")]
    [SerializeField] public bool stopByTapping = false;
    [Tooltip("The duration for the hit text to be visible")]
    [SerializeField] private float hitTextDuration = 0.75f;
    [Tooltip("The starting speed of the needle")] 
    [SerializeField] private float minSpinSpeed = 130f;
    [Tooltip("The max speed of the needle")]
    [SerializeField] private float maxSpinSpeed = 200f;
    [SerializeField] AnimationCurve spinSpeedCurve;
    [Tooltip("This is the amount of STR/PWR/SOUL that will be expended when performing an attack that passes this many cardinal directions on the compass")]
    [SerializeField] private int[] powerRequiredForAttacks;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    [Tooltip("The current angle the sword needle is pointing in")]
    private float swordAngle = 0f;
    [Tooltip("Used to track when the attack bar is in progress")]
    private bool attackBarActive;
    private int spinStartIndex = 0;
    private SpinDirection currentSpinDirection;
    private enum RingState { notStarted, spinning, finish }
    private RingState currentState = RingState.notStarted;
    
    [Tooltip ("How fast the sword needle travels around the compass")]
    private float currentSpinSpeed;
    [Tooltip("Used to keep track of when teh attack bar started")]
    private bool hasInitialized;
    [Tooltip("Tracks the amount the compass has spun (positive or negative) to determine what way to swing the sword.")]
    private float clampedTotalSpin = 0f;
    private float totalSpin = 0f;
    private float nearestAngleToSword;
    private float distanceFromNearestAngle;
    private SwordSwingAnimationHandler swordSwingAnimator;
    private BattleCameraManager battleCameraManager;


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

    [Tooltip("Percent damage dealt after you hit the \"good\" zone")]
    [SerializeField] private float goodDamageMultiplier = 0.75f;
    //multiplier to use for attacks
    private float currentDamageMultiplier;
    //identifier for the compass multiplier
    private const string Mod_DamageMult = "Mod_DamageMult";

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [Tooltip ("Reference to the player so we can freeze them when attacking")]
    private Char_Battle_Player player;
    
    [Tooltip("The image that represents the sword angle on the attack compass")]
    [SerializeField]private Image needleImage;
    
    [Tooltip("Keep track of the active hit text coroutine so we make sure only one is running")]
    private Coroutine showHitTextCoroutine;
    [Tooltip("The 4 images that are used to fill the 4 bars for this hit angle")]
    [SerializeField] private Image[] goodBarImages, perfectBarImages;
    [Tooltip("Text used to display how good the hit angle was")]
    [SerializeField] private TMP_Text hitText;
    //The object used for generated sword swing attacks.
    [SerializeField] private GameObject defaultAttackObject;
    private Coroutine resetRoutine;
    [Tooltip("")] 
    [SerializeField] private Image centerFill;
    [Tooltip("")] 
    [SerializeField] private Image powerMask1;
    [Tooltip("")] 
    [SerializeField] private Image powerMask2;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
   public void Start()
    {
        player = GameInstance.Playerbody as Char_Battle_Player;
        swordSwingAnimator = player.GetComponentInChildren<SwordSwingAnimationHandler>();
        battleCameraManager = FindObjectOfType<BattleCameraManager>();
    } 

    public void OnEnable()
    {
        ResetCompass();
    }

    public void Update()
    {
        // Update the needle based on the sword angle
        needleImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0f, -swordAngle));
        
        // Update how much our current power can actually swing the sword
        UpdatePowerMeterBasedOnAvailablePower();
        
        // Detect activation
        if (!attackBarActive)
        {
            // If we aren't in the process of attacking, update the needle direction to match the player's direction
            SetNeedleDirection(player.facingDirection);
            
            // Start the attack timer on first press
            if (GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                currentSpinDirection = SpinDirection.Left;
                Initialize();
            }
            else if (GameInstance.Inputs.Action.WasPressedThisFrame())
            {
                currentSpinDirection = SpinDirection.Right;
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


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Resets the attack compass so another attack can be performed
    /// </summary>
    private void ResetCompass()
    {
        SetupRingColors();
        
        // Unhide the power meters
        powerMask1.enabled = true;
        powerMask2.enabled = true;
        
        
        // Reset some other values that may still be filled out from a previous attack
        centerFill.gameObject.transform.localRotation = Quaternion.Euler (0, 0, -swordAngle);
        centerFill.fillAmount = 0f;
        clampedTotalSpin = 0f;
        totalSpin = 0f;
        resetRoutine = null;
    }
    
    /// <summary>
    /// Called when reset, Adjust the hit bar images to match the defined hit angles
    /// </summary>
    private void SetupRingColors ()
    {
        const int hitBarCount = 4;

        // Loop through each hit bar
        for (int i = 0; i < hitBarCount; i++)
        {
            var someCalculation = (90 * i);
            var goodRotation = Quaternion.Euler(0, 0, goodAngle + someCalculation);
            var perfectRotation = Quaternion.Euler (0, 0, perfectAngle + someCalculation);
            
            // Adjust their rotation around the compass
            goodBarImages[i].gameObject.transform.localRotation = goodRotation;
            perfectBarImages[i].gameObject.transform.localRotation = perfectRotation;
            
            // Adjust their fill amount based on the success angles
            goodBarImages[i].fillAmount = (goodAngle * 2f) / 360;
            perfectBarImages[i].fillAmount = (perfectAngle * 2f) / 360;
        }
    }

    //_direction could be null because facingDireciton was sometimes diagonal, using this in below method as failsafe? ~Erry
    private Direction lastValidFacingDireciton = Direction.North;

    /// <summary>
    /// Sets the fill amount and rotation of the power meter rings to match the current facing direction and power level
    /// </summary>
    private void UpdatePowerMeterBasedOnAvailablePower()
    {
        // Rotate the power meters to the direction of the sword
        player.facingDirection.TryConvertToDirection(out Direction? _direction);

        //_direction could be null because facingDireciton was sometimes diagonal, using this as failsafe? ~Erry
        if (_direction == null) _direction = lastValidFacingDireciton;
        else lastValidFacingDireciton = _direction.Value;

        powerMask1.transform.localRotation = Quaternion.Euler(_direction.Value.Info().attackCompassFillRotationX);
        powerMask2.transform.localRotation = Quaternion.Euler(_direction.Value.Info().attackCompassFillRotationX);
        
        // Set the fill amount based on the available power
        float _fillAmount = 0;
        if (player.Stats.power >= 40) _fillAmount = 1f;     // Full Slash (360)
        if (player.Stats.power >= 30) _fillAmount = 0.75f;  // Three-Quarts Slash (270)
        if (player.Stats.power >= 20) _fillAmount = 0.5f;   // Half Slash (180)
        if (player.Stats.power >= 10) _fillAmount = 0.25f;  // Quarter Slash (90 turn)
        powerMask1.fillAmount = _fillAmount;
        powerMask2.fillAmount = _fillAmount;
    }
    
    /// <summary>
    /// Called when not currently attacking, updates the needle to the player's sword direction
    /// </summary>
    private void SetNeedleDirection(Vector2 _facingDirection)
    {
        spinStartIndex = (int)swordAngle / 90;
        
        //Convert vector2 into a Direction, and set compass direction to degrees rotation of that direction rotated 18- degrees
        if (_facingDirection.TryConvertToDirection(out Direction? direction))
        {
            swordAngle = direction.Value.Info().turned180.Info().degreesRotation;
        }
    }
    
    /// <summary>
    /// Freeze the player movement and enable inputs for the attack bar
    /// </summary>
    private void Initialize()
    {
        // Keep from calling this function more than once (Need to call reset to call this function again)
        if (hasInitialized) return;
        hasInitialized = true;
        
        // Setup the ring state
        currentSpinSpeed = minSpinSpeed;
        currentState = RingState.spinning;
        
        // hide the power bar that's not relevant to the current spin
        if (currentSpinDirection == SpinDirection.Left) powerMask1.enabled = false;
        if (currentSpinDirection == SpinDirection.Right) powerMask2.enabled = false;

        // Freeze player and enable inputs
        player.canMove = false;
        attackBarActive = true;
    }
    
    /// <summary></summary>
    private void DoSpinState ()
    {
        //Set animation to start pullback of sword
        swordSwingAnimator.swingState = SwordSwingAnimationHandler.SwingState.Pullback;
        swordSwingAnimator.attackStartDirection = lastValidFacingDireciton;
        swordSwingAnimator.spinDireciton = currentSpinDirection;

        if (stopByTapping)
        {
            if (GameInstance.Inputs.Action.WasPressedThisFrame() || GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                FinishSpin ();
                return;
            }
        }
        else
        {
            if (GameInstance.Inputs.Action.WasReleasedThisFrame() || GameInstance.Inputs.Interact.WasReleasedThisFrame())
            {
                FinishSpin();
                return;
            }
        }

        float spinAmount = 0f;
        if (currentSpinDirection == SpinDirection.Left)
        {
            spinAmount = -currentSpinSpeed * Time.deltaTime;
        }
        if (currentSpinDirection == SpinDirection.Right)
        {
            spinAmount = currentSpinSpeed * Time.deltaTime;
        }

        //Set spin animation degrees of rotation
        swordSwingAnimator.spinDegreesRotation = spinAmount;

        swordAngle += spinAmount;
        clampedTotalSpin += spinAmount;
        totalSpin += spinAmount;

        float percent = Mathf.Abs (totalSpin) / 360;
        float t = spinSpeedCurve.Evaluate (percent);

        //Update sword-pullback and camera zoom based on factor
        swordSwingAnimator.swordPullbackFactor = t;
        battleCameraManager.UpdateCameraOnAttack(t);

        currentSpinSpeed = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, t);


        //Clamps the totalSpin, but only if it goes far enough past 360 that we've looped around to a 90-degrees swing again.
        //The cutoff is 45 degrees past 360, since that would clamp to 90 degrees.
        if (clampedTotalSpin > 360 + 45)
        {
            clampedTotalSpin -= 360;
        }
        if (clampedTotalSpin < -360 - 45)
        {
            clampedTotalSpin += 360;
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
    
    
    
    
    /// <summary>
    /// Coroutine to delay the reset of the compass after an attack, to add cooldown before the player can attack again
    /// </summary>
    private IEnumerator CoReset()
    {
        currentState = RingState.finish;
        swordAngle = nearestAngleToSword;
        yield return new WaitForSeconds(0.25f);
        ResetCompass();
    }

    
    /// <summary> Resets the minigame. </summary>
    private void OnAttackDone()
    {
        if (resetRoutine != null)
        {
            //StopCoroutine (resetRoutine);
        }
        resetRoutine = StartCoroutine(CoReset());
    }
    
    private void PlaceCenterFill ()
    {
        centerFill.fillAmount = Mathf.Abs(clampedTotalSpin) / 360f;
        if (clampedTotalSpin > 0)
        {
            //print("POSITIVE " + spinStartIndex * 90);
            centerFill.transform.localRotation = Quaternion.Euler(new Vector3(0, 0f, -spinStartIndex*90));
            return;
        }
        if (clampedTotalSpin < 0f)
        {
            //print("NEGATIVE");
            centerFill.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0f, (-spinStartIndex * 90)-clampedTotalSpin));
        }
    }

    /// <summary> Ends sword spinning and calculates the direction it was pointing.</summary>
    private void FinishSpin ()
    {
        if (Mathf.Abs(clampedTotalSpin) <= 45) //When swing is less than 45
        {
            if (stopByTapping)
            {
                ShowHitText("Miss!");
                FailAttack();
            }
            else
            {
                ShowHitText("Hold to attack");
                FailAttack(advanceTurn: false);
            }
            return;
        }

        distanceFromNearestAngle = Mathf.Abs(Mathf.DeltaAngle(swordAngle, Direction.North.Info().degreesRotation));
        nearestAngleToSword = Direction.North.Info().degreesRotation;
        DirectionUtility.ForEachDirection((direction, directionInfo) =>
        {
            var test = Mathf.Abs(Mathf.DeltaAngle(swordAngle, directionInfo.degreesRotation));
            if (test < distanceFromNearestAngle)
            {
                distanceFromNearestAngle = test;
                nearestAngleToSword = directionInfo.degreesRotation;
            }
        });

        if (distanceFromNearestAngle < perfectAngle)
        {
            ShowHitText("Perfect!");
            currentDamageMultiplier = 1f;
        }
        else if (distanceFromNearestAngle < goodAngle) {
            ShowHitText("Good");
            currentDamageMultiplier = goodDamageMultiplier;
        }
        else
        {
            ShowHitText("Miss!");
            FailAttack();
            return;
        }

        ClampTotalSpin();
        //Try consuming amount of power corresponding to size of spin
        //If there's not enough power, the attack fails.
        var index = Mathf.Abs((int)clampedTotalSpin)-1;
        print($"{index} uses {powerRequiredForAttacks[index]}");
        if (player.Stats.TryUsePower(powerRequiredForAttacks[index]) == false)
        {
            ShowHitText("POWER TOO LOW!");
            FailAttack();
            return;
        }

        //Set animation to start spinning
        swordSwingAnimator.StartSpin(spinStartIndex, Mathf.RoundToInt(clampedTotalSpin));

        ExecuteAttack();
        centerFill.fillAmount = 0;
    }    

    private void ClampTotalSpin ()
    {
        clampedTotalSpin = Mathf.RoundToInt (clampedTotalSpin / 90f);
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

    /// <summary>
    /// Called by the battle system
    /// </summary>
    public void ReEnableCompassInputs()
    {
        // Reset activation and initialization flags
        currentState = RingState.notStarted;
        attackBarActive = false;
        hasInitialized = false;
        battleCameraManager.GoBackHome();
    }

    private void ExecuteAttack()
    {
        var sequence = new AttackSequence ();
        sequence.attacks = new List<AttackElement> ();
        int n = spinStartIndex * 2;
        int increment = MathF.Sign (clampedTotalSpin);
        //Generate an attack by looping through the swingPattern
        for (int i = 0; i < Mathf.Abs(clampedTotalSpin*2)+1; i++)
        {
            AttackElement attack = new AttackElement ();
            attack.position = swingPattern[n];
            attack.visualEffect = defaultAttackObject;
            attack.pushing = true;
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
        //Apply minigame damage multiplier, perfom the attack and then remove the multiplier.
        player.Stats.attack.ModifyStatWith (Mod_DamageMult, NumberModifierType.Multiply, currentDamageMultiplier);
        player.PerformGeneratedAttack();
        player.Stats.attack.UnmodifyStatWith (Mod_DamageMult);

        OnAttackDone();

        if (Mathf.Abs(clampedTotalSpin) > 2)
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.slash3, 1);
        }
        else if (Mathf.Abs (clampedTotalSpin) > 1)
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.slash2, 1);
        }
        else if (Mathf.Abs (clampedTotalSpin) > 0)
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.slash1, 1);
        }
        else
        {
            GI_AudioManager.Instance.PlaySlashClip (GI_AudioManager.Instance.failBuzz, 1);
        }
    }

    private void FailAttack (bool advanceTurn = true)
    {
        GI_AudioManager.Instance.PlayClip (GI_AudioManager.Instance.failBuzz);
        centerFill.fillAmount = 0;
        OnAttackDone();
        if (advanceTurn)
        {
            swordSwingAnimator.FailAttack();
            player.SkipTurn();
        }
        else
        {
            swordSwingAnimator.swingState = SwordSwingAnimationHandler.SwingState.None;
            ReEnableCompassInputs();
        }
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
            return;
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
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
