using UnityEngine;
using DG.Tweening;

public class Anim_ScaleIn : MonoBehaviour
{
    [SerializeField] private float time = 1f;
    [SerializeField] Ease easing;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, time).SetEase(easing);
    }
}
