using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingAnimationHandler : MonoBehaviour
{
    [SerializeField] private Transform visualContainer_default;
    [SerializeField] private Transform visualContainer_swingSword;

    public Direction attackStartDirection;
    public SpinDirection spinDireciton;
    public SwingState swingState;

    public enum SwingState { None, Pullback, Spin, Recoil }

    [Range(0f, 1f)]
    public float swordPullbackFactor = 0f;

    public void Update()
    {
        switch (swingState)
        {
            case SwingState.None: return;
            case SwingState.Pullback: OnPullback(); break;
            case SwingState.Spin: OnSpin(); break;
            case SwingState.Recoil: OnRecoil(); break;
        }
    }

    public void OnPullback()
    {

    }
    public void OnSpin()
    {

    }
    public void OnRecoil()
    {

    }
}
