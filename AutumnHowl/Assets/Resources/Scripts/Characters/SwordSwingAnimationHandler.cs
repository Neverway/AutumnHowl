using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingAnimationHandler : MonoBehaviour
{
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

    public void Update()
    {
        if (visualContainer_swingSword != null)
            visualContainer_swingSword.gameObject.SetActive(swingState != SwingState.None);
        if (visualContainer_default != null )
            visualContainer_default.gameObject.SetActive(swingState == SwingState.None);

        switch (swingState)
        {
            case SwingState.None: OnNone(); return;
            case SwingState.Pullback: OnPullback(); break;
            case SwingState.Spin: OnSpin(); break;
            case SwingState.Recoil: OnRecoil(); break;
        }
    }
    public void OnNone()
    {
    }
    public void OnPullback()
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
    public void OnSpin()
    {
        swordTrail.gameObject.SetActive(true);
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
    public void OnRecoil()
    {

    }
}
