using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwordSwingAnimationHandler : MonoBehaviour
{
    public ParticleEffect OnHitParticle;

    [SerializeField] private Animator animator;

    [SerializeField] private string[] pullbackDirectionStates;
    [SerializeField] private string spinState;

    [Space]
    [SerializeField] private Transform visualContainer_default;
    [SerializeField] private Transform visualContainer_swingSword;
    [SerializeField] private Transform swordTrail;

    [Space]
    [Header("Input controls")]
    public SwingState swingState;
    public enum SwingState { None, Pullback, Spin, Recoil }
    [Space]
    public Direction attackStartDirection;
    public SpinDirection spinDireciton;
    [Range(0f, 1f)] public float swordPullbackFactor = 0f;
    public float spinDegreesRotation;

    public bool useTrailOutsideOfSpin = false;

    public void Update()
    {
        
        //Debug.DrawLine(transform.position + dir, dir, Color.cyan);

        //If swing state is None, use default visuals, otherwise switch to swing visuals!
        if (visualContainer_swingSword != null) visualContainer_swingSword.gameObject.SetActive(swingState != SwingState.None);
        if (visualContainer_default != null ) visualContainer_default.gameObject.SetActive(swingState == SwingState.None);

        //Use sword trail if spinning or "useTrailOutsideOfSpin" flag is true
        swordTrail.gameObject.SetActive(useTrailOutsideOfSpin || swingState == SwingState.Spin);

        //Do logic for each swing state
        switch (swingState)
        {
            case SwingState.None: OnNone(); return;
            case SwingState.Pullback: OnPullback(); break;
            case SwingState.Spin: OnSpin(); break;
            case SwingState.Recoil: OnRecoil(); break;
        }
    }
    public void OnDrawGizmos()
    {
        Vector3 dir = Quaternion.AngleAxis(spinDegreesRotation, Vector3.back) * Vector3.up;
        dir = dir.normalized * 0.5f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, dir);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay (transform.position, attackStartDirection.Info().directionVector3);
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
        string pullbackState = pullbackDirectionStates[(int)direcitonToUse];
        
        float factor = Mathf.Min(swordPullbackFactor, 0.99f);
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
        animator.Play(spinState, 0, factor);
        animator.speed = 0f;

    }
    private void OnRecoil()
    {

    }

    /// <summary>Starts an entire spin coroutine for the animation of spinning your attack</summary>
    /// <param name="swordStartDirectionIndex">For the sword: 0 = NORTH, 1 = EAST, 2 = SOUTH, 4 = WEST</param>
    /// <param name="swordSwingDirectionIndexDelta">Change in that index. Positive = spin RIGHT, Negative = spin LEFT</param>
    public void StartSpin(int swordStartDirectionIndex, int swordSwingDirectionIndexDelta)
    {
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
        StartCoroutine(CoSpinning(degreesStart, degreesEnd));
    }

    //==================== [ Spinning animation coroutine ] =======================================================================

    private float hitStunTimer;
    private Vector3 hitDirection;

    private void RegisterHit() => hitStunTimer = 0.15f;
    public void RegisterHit(AttackElement attack, Vector3 hitPosition) 
    {
        Debug.Log("Erry: Hitstun!!!");
        hitDirection = (hitPosition - transform.position).normalized;
        Debug.DrawLine(transform.position, hitPosition, Color.red, 1f);

        RegisterHit();
        Instantiate(OnHitParticle, hitPosition, Quaternion.identity).animationSpeed.baseFactor = 1.4f;

    }
    private bool recoiling;
    public void RegisterRecoil()
    {
        recoiling = true;
    }
    private IEnumerator CoSpinning(float startAngle, float endAngle)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            EditorApplication.isPaused = true;

        float secondsPer90Degrees = 0.25f;
        float timeScale_PullbackIntoSpin = 3f + (Mathf.Abs(startAngle - endAngle) / 360 * 2);
        float timeScale_SpinToRest = 1f;

        float secondsToSpin = (Mathf.Abs(startAngle - endAngle) / 90f) * secondsPer90Degrees;
        float timeStart = Time.time;
        hitStunTimer = 0f;
        recoiling = false;

        //================================================== [Lower sword from pullback for a few frames to enter spin]
        if (swingState == SwingState.Pullback)
        {
            Debug.Log("Erry: ATTACK ANIMATION: Pullback down into spin");
            swordPullbackFactor = Mathf.Min(swordPullbackFactor, 0.8f);
            while (true)
            {
                swordPullbackFactor -= Time.deltaTime * timeScale_PullbackIntoSpin;

                if (swordPullbackFactor > 0.25f) yield return null;
                else break;
            }
        }

        //============================================================================== [Start the spinning animation]
        Debug.Log("Erry: ATTACK ANIMATION: Start spin");
        swingState = SwingState.Spin;
        while (true)
        {
            while(true) //Apply hitstun to spin animation
            {
                if (hitStunTimer > 0f) yield return null;
                else break;
                if (hitDirection != Vector3.zero)
                {
                    Debug.Log($"Erry: hitstun calculatedAngle {hitDirection.DirectionTo2DAngle()}");
                    
                    spinDegreesRotation = hitDirection.DirectionTo2DAngle() + 180;
                }

                hitStunTimer -= Time.deltaTime;
            }

            float t = Mathf.InverseLerp(timeStart, timeStart + secondsToSpin, Time.time); //0 -> 1 time of spin
            spinDegreesRotation = Mathf.Lerp(startAngle, endAngle, t);

            if (t < 1f) yield return null;
            else break;
        }

        //====================================================================== [Lower sword from spin back to ground]
        Debug.Log("Erry: ATTACK ANIMATION: Lower sword to rest");
        attackStartDirection = Mathf.RoundToInt(endAngle / 90).DirectionIndexToDirection();
        swingState = SwingState.Pullback;
        swordPullbackFactor = 0.3f;
        while (true)
        {
            swordPullbackFactor -= Time.deltaTime * timeScale_SpinToRest;
            swordPullbackFactor = Mathf.Max(swordPullbackFactor, 0f);
            if (swordPullbackFactor > 0f) yield return null;
            else break;
        }

        //===================================================================================== [End animation control]
        Debug.Log("Erry: ATTACK ANIMATION: End");
        swingState = SwingState.None;
        yield break;
    }
}
