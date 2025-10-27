using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class SwordSwingAnimationHandler : MonoBehaviour
{
    //==================== [ Constants ] =======================================================================

    //------- For animating small transition from Sword-pullback state to Sword-spin state  (sword lowering) ---------------
    public const float PULLBACK_TO_SPIN_TIMESCALE_MIN = 3f; //Lowering sword speed when turning 0 degrees
    public const float PULLBACK_TO_SPIN_TIMESCALE_MAX = 6f; //Lowering sword speed when turning 360 degrees
    public const float PULLBACK_TO_SPIN_FACTOR_FROM = 0.8f; //Pullback factor to cap on start of lowering sword
    public const float PULLBACK_TO_SPIN_FACTOR_TO = 0.25f; //Pullback to reach to end lowering sword and start spin

    //------- For handling when the player is spinning the sword -----------------------------------------------------------
    public const float HITSTUN_SECONDS = 0.13f; //Seconds to freeze the rotation for when hit happens
    public const float SPIN_90DEGREES_SECONDS = 0.18f; //Seconds per 90 degrees of spin

    //------- For animation small transition from Sword-spin to ending the animation (resting the sword) -------------------
    public const float SPIN_TO_REST_TIMESCALE = 1f; //Speed for resting the sword
    public const float SPIN_TO_REST_FACTOR_FROM = 0.3f; //Pullback factor to start at for rest animation
    public const float SPIN_TO_REST_FACTOR_TO = 0f; //Pullback factor to reach to end rest animation


    public const float FAIL_ATTACK_TIMESCALE = 1.4f; //Pullback factor to reach to end rest animation


    //==================== [ Reference fields ] =======================================================================
    public ParticleEffect OnHitParticle;

    [SerializeField] private Animator animator;
    [SerializeField] private string animator_swingStateName;
    [SerializeField] private string animator_victoryDanceStateName;
    [SerializeField] private string[] animator_pullbackDirectionStateNamess;
    [SerializeField] private string[] animator_failDirectionStateNamess;
    [Space]
    [SerializeField] private Transform visualContainer_default;
    [SerializeField] private Transform visualContainer_swingSword;
    [SerializeField] private Transform swordTrail;
    [SerializeField] private Transform buryGround;


    //==================== [ Controls fields ] =======================================================================

    [Space, Header("Input controls")]
    public SwingState swingState;
    public enum SwingState { None, Pullback, Spin, Victory, Other }
    private Direction _attackStartDirection;
    public Direction attackStartDirection { get => (Direction)((int)_attackStartDirection % 4); set => _attackStartDirection = value; }
    public SpinDirection spinDireciton;
    [Range(0f, 1f)] public float swordPullbackFactor = 0f;
    public float spinDegreesRotation;
    public bool useTrailOutsideOfSpin = false;
    public float victoryDanceSpeed = 0.25f;

    //==================== [ Controller Logic ] =======================================================================
    public void Update()
    {
        //If swing state is None, use default visuals, otherwise switch to swing visuals!
        if (visualContainer_swingSword != null) visualContainer_swingSword.gameObject.SetActive(swingState != SwingState.None);
        if (visualContainer_default != null ) visualContainer_default.gameObject.SetActive(swingState == SwingState.None);
        if (buryGround != null ) buryGround.gameObject.SetActive(swingState == SwingState.Victory);

        //Use sword trail if spinning or "useTrailOutsideOfSpin" flag is true
        swordTrail.gameObject.SetActive(useTrailOutsideOfSpin || swingState == SwingState.Spin);

        //Do logic for each swing state
        switch (swingState)
        {
            case SwingState.None: OnNone(); return;
            case SwingState.Pullback: OnPullback(); break;
            case SwingState.Spin: OnSpin(); break;
            case SwingState.Victory: OnVictory(); break;
            case SwingState.Other: OnOther(); break;
        }
    }
    public void OnDrawGizmos()
    {
        try
        {
            Vector3 dir = Quaternion.AngleAxis(spinDegreesRotation, Vector3.back) * Vector3.up;
            dir = dir.normalized * -0.5f;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, dir);

            Gizmos.color = Color.yellow;
            Vector3 attackDireciton = attackStartDirection.Info().directionVector3;
            Gizmos.DrawRay(transform.position + (Vector3.one * 0.01f), attackDireciton * 0.5f);
        }
        catch { }
    }

    private void OnNone()
    {
    }
    private void OnPullback()
    {
        swordTrail.gameObject.SetActive(false);
        Direction direcitonToUse = attackStartDirection;
        switch (spinDireciton)
        {
            case SpinDirection.Left:
                visualContainer_swingSword.localScale = new Vector3(1f, 1f, 1f);
                break;
            case SpinDirection.Right:
                visualContainer_swingSword.localScale = new Vector3(-1f, 1f, 1f);
                switch (attackStartDirection)
                {
                    case Direction.East: direcitonToUse = Direction.West; break;
                    case Direction.West: direcitonToUse = Direction.East; break;
                }
                break;
            default:
                return;
        }
        string pullbackState = animator_pullbackDirectionStateNamess[Mathf.Abs((int)direcitonToUse % 4)];

        //This is a hacky attempt to raise the start value when I should really just change the animations.
        //To see what this does, compare "y = x" for original 0 to 1 with "y = x^(x+0.45)" for the new 0 to 1 range
        float factor = Mathf.Pow(swordPullbackFactor, swordPullbackFactor + 0.45f);

        factor = Mathf.Min(factor, 0.99f); //if it equals 1 exactly, it loops to beginning

        animator.Play(pullbackState, 0, factor);
        animator.speed = 0f;
    }
    private void OnSpin()
    {
        float degrees = spinDegreesRotation;
        switch (spinDireciton)
        {
            case SpinDirection.Left:
                visualContainer_swingSword.localScale = new Vector3(1f, 1f, 1f);
                degrees = -degrees;
                break;
            case SpinDirection.Right:
                visualContainer_swingSword.localScale = new Vector3(-1f, 1f, 1f);
                break;
            default:
                return;
        }
        float factor = (degrees / 360f) % 1f;

        animator.Play(animator_swingStateName, 0, factor);
        animator.speed = 0f;

    }
    private void OnVictory()
    {
        animator.Play(animator_victoryDanceStateName, 0, Time.time * victoryDanceSpeed);
        animator.speed = 0f;
    }
    private void OnOther()
    {

    }

    public void FailAttack()
    {
        StartCoroutine(CO_PreformFailedAttack());
    }

    private IEnumerator CO_PreformFailedAttack()
    {
        swingState = SwingState.Other;
        Direction direcitonToUse = attackStartDirection;
        switch (spinDireciton)
        {
            case SpinDirection.Left:
                visualContainer_swingSword.localScale = new Vector3(1f, 1f, 1f);
                break;
            case SpinDirection.Right:
                visualContainer_swingSword.localScale = new Vector3(-1f, 1f, 1f);
                switch (attackStartDirection)
                {
                    case Direction.East: direcitonToUse = Direction.West; break;
                    case Direction.West: direcitonToUse = Direction.East; break;
                }
                break;
            default:
                yield break;
        }

        string failState = animator_failDirectionStateNamess[(int)direcitonToUse];
        float factor = Mathf.Min(swordPullbackFactor, 0.99f); //if it equals 1 exactly, it loops to beginning
        float animationTime = 0f;

        while (true)
        {
            animator.Play(failState, 0, animationTime);
            animator.speed = 0f;

            animationTime += Time.deltaTime * FAIL_ATTACK_TIMESCALE;
            if (animationTime < 1f) yield return null;
            else break;
        }
        swingState = SwingState.None;
    }


    //==================== [ Spinning animation setup ] =======================================================================
    //--- Hit Registering ----------------------------
    float hitStunTimer; Vector3 hitDirection;
    private void RegisterHit() => hitStunTimer = HITSTUN_SECONDS;
    public void RegisterHit(AttackElement attack, Vector3 hitPosition)
    {
        Debug.Log("Erry: Hitstun!!!");
        hitDirection = (hitPosition - transform.position).normalized;
        Debug.DrawLine(transform.position, hitPosition, Color.red, 1f);

        RegisterHit();
        Instantiate(OnHitParticle, hitPosition + Vector3.up * 0.4f, Quaternion.identity);

    }
    
    //--- Recoil Registering ----------------------------
    bool startRecoil; Direction toRecoilTo;
    public void RegisterRecoil(Direction returnToDirection)
    {
        Debug.Log("Erry: REGISTERED RECOIL");
        startRecoil = true;
        toRecoilTo = returnToDirection;
    }

    /// <summary>Starts an entire spin coroutine for the animation of spinning your attack</summary>
    /// <param name="swordStartDirectionIndex">For the sword: 0 = NORTH, 1 = EAST, 2 = SOUTH, 4 = WEST</param>
    /// <param name="swordSwingDirectionIndexDelta">Change in that index. Positive = spin RIGHT, Negative = spin LEFT</param>
    public void StartSpin(int swordStartDirectionIndex, int swordSwingDirectionIndexDelta)
    {
        #if UNITY_EDITOR
        //Debug pause player when holding shift
        if (Input.GetKey(KeyCode.LeftShift)) EditorApplication.isPaused = true;
        #endif

        //Set spin direction based on swing index delta (positive is right (clockwise), negative is left (counter-clock))
        if (swordSwingDirectionIndexDelta > 0) 
            spinDireciton = SpinDirection.Right;
        else spinDireciton = SpinDirection.Left;

        //Rotate indexes 180 degrees because animator uses player facing direction instead of 
        swordStartDirectionIndex += 2;

        //Translate indexes to degrees start and end
        float degreesStart = swordStartDirectionIndex * 90f;
        float degreesEnd = degreesStart + (swordSwingDirectionIndexDelta * 90f);

        //Start the whole spin animation
        hitStunTimer = 0f;
        startRecoil = false;
        StartCoroutine(Co_PreformSpinAnimation(degreesStart, degreesEnd));
    }


    //==================== [ Spinning animation main control ] =======================================================================

    private IEnumerator Co_PreformSpinAnimation(float startAngle, float endAngle)
    {
        //Cache time started and seconds to preform spin
        float secondsToSpin = (Mathf.Abs(startAngle - endAngle) / 90f) * SPIN_90DEGREES_SECONDS;
        float timeStarted = Time.time;

        //Lower sword from pullback for a few frames to enter spin
        Debug.Log("Erry: SpinPhase: Lower sword into spin");
        yield return SpinPhase_LowerSwordIntoSpin(startAngle, endAngle);

        //Start the spinning animation
        Debug.Log("Erry: SpinPhase: Spin");
        yield return SpinPhase_Spin(startAngle, endAngle, timeStarted, secondsToSpin);

        if (startRecoil) //If a recoil occurred during spin, retry the spin in the other direction, but with new angle targets and time
        {
            startRecoil = false;
            if (spinDireciton == SpinDirection.Left) spinDireciton = SpinDirection.Right;
            else if (spinDireciton == SpinDirection.Right) spinDireciton = SpinDirection.Left;

            spinDegreesRotation = Mathf.Abs(spinDegreesRotation % 360);
            startAngle = spinDegreesRotation;
            endAngle = toRecoilTo.Info().degreesRotation;
            Debug.Log($"BEFORE: starts: {startAngle}, ends: {endAngle}");
            if (endAngle - startAngle > 180) endAngle -= 360; //Make sure to use the shortest distance of angles
            if (endAngle - startAngle < -180) endAngle += 360;
            Debug.Log($"AFTER: starts: {startAngle}, ends: {endAngle}");

            secondsToSpin = (Mathf.Abs(startAngle - endAngle) / 90f) * SPIN_90DEGREES_SECONDS;
            timeStarted = Time.time;

            Debug.Log("Erry: SpinPhase: Recoil spin");
            yield return SpinPhase_Spin(startAngle, endAngle, timeStarted, secondsToSpin);
        }

        //End spin by resting sword back to ground
        Debug.Log("Erry: SpinPhase: Bring sword to rest");
        yield return SpinPhase_BringSwordToRest(endAngle);
    }
    private IEnumerator SpinPhase_LowerSwordIntoSpin(float startAngle, float endAngle)
    {
        //If you have not entered this phase pulling your sword back, no need to lower the sword
        if (swingState != SwingState.Pullback) yield break;

        //Get timescale for this animation phase
        float timeScale =
            Mathf.Lerp(PULLBACK_TO_SPIN_TIMESCALE_MIN, PULLBACK_TO_SPIN_TIMESCALE_MAX, (Mathf.Abs(startAngle - endAngle) / 360));

        //Start with sword trail and move down sword to starting lowering position
        useTrailOutsideOfSpin = true;
        swordPullbackFactor = Mathf.Min(swordPullbackFactor, PULLBACK_TO_SPIN_FACTOR_FROM);
        while (true)
        {
            //If a hitstun is active, speed up animation significantly to get to hitstun faster
            if (hitStunTimer > 0f) timeScale = PULLBACK_TO_SPIN_TIMESCALE_MAX * 1.5f;

            //Lower sword
            swordPullbackFactor -= Time.deltaTime * timeScale;
            OnPullback();

            //Stay until sword has lowered far enough
            if (swordPullbackFactor > PULLBACK_TO_SPIN_FACTOR_TO) 
                yield return null;
            else break;
        }
        useTrailOutsideOfSpin = false;

    }
    private IEnumerator SpinPhase_Spin(float startAngle, float endAngle, float timeStarted, float secondsToSpin)
    {
        swingState = SwingState.Spin;
        while (true)
        {
            //Wait for any hitstuns (if there are any)
            yield return SpinPhase_Hitstun();

            if (startRecoil) yield break; //Exit if need to start recoil

            //Convert current time to degrees rotation
            float t = Mathf.InverseLerp(timeStarted, timeStarted + secondsToSpin, Time.time); //0 -> 1 time of spin
            spinDegreesRotation = Mathf.Lerp(startAngle, endAngle, t); //tween degrees rotation on 0 -> 1 time
            OnSpin();
            //Stay until spin time has ended
            if (t < 1f) yield return null;
            else break;
        }
        swingState = SwingState.None;
    }
    private IEnumerator SpinPhase_Hitstun()
    {
        while (true)
        {
            //Wait while hitstun is active
            if (hitStunTimer > 0f) yield return null;
            else break;

            //Set rotation to direction of hit
            if (hitDirection != Vector3.zero)
                spinDegreesRotation = hitDirection.DirectionTo2DAngle() + 180;
            //Wait for 
            hitStunTimer -= Time.deltaTime;
        }
    }

    private IEnumerator SpinPhase_BringSwordToRest(float endAngle)
    {
        //Set facing direction based on ending angle
        attackStartDirection = Mathf.RoundToInt(endAngle / 90).DirectionIndexToDirection();
        //Switch to pull
        swingState = SwingState.Pullback;
        swordPullbackFactor = SPIN_TO_REST_FACTOR_FROM;
        while (true)
        {
            swordPullbackFactor -= Time.deltaTime * SPIN_TO_REST_TIMESCALE;
            swordPullbackFactor = Mathf.Max(swordPullbackFactor, 0f);

            OnPullback();

            if (swordPullbackFactor > SPIN_TO_REST_FACTOR_TO) yield return null;
            else break;
        }
        //End swing state
        swingState = SwingState.None;
    }


}
