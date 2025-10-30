using UnityEngine;
using UnityEngine.Events;

//This is obnoxiously named on purpose so you can find it in dropdowns quicker
public class UNITY___EVENT___WRAPPER : MonoBehaviour
{
    public UnityEvent OnInvoke;
    public void INVOKE___UNITY___EVENT___WRAPPER() => OnInvoke?.Invoke();
}
