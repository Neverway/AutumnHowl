using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>Keeps only 1 child active at a timeo</summary>
public class Utility_ChildSelector : MonoBehaviour
{
    public Transform SelectedChild { get; private set; }
    public bool startWithRandomChildSelected;
    public bool invokeOnStartChildSelect;

    [Tooltip("Called BEFORE any select child function is called")]
    public UnityEvent beforeSelectChild = new UnityEvent();

    [Tooltip("Called AFTER any select child function is called")]
    public UnityEvent afterSelectChild = new UnityEvent();

    public int SelectedChildIndex => (SelectedChild == null) ? -1 : SelectedChild.GetSiblingIndex();

    private bool suppressInvokes = false;

    public virtual void Start()
    {
        suppressInvokes = !invokeOnStartChildSelect;

        if (startWithRandomChildSelected) Select_RandomChild();
        else if (SelectedChild == null) Select_FirstChild();
        else Select_Child(SelectedChild);

        suppressInvokes = false;
    }

    public void UpdateChildSelect_WithoutInvoke()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(child == SelectedChild);
    }
    private void SetSelect(Transform obj)
    {
        if (!suppressInvokes) beforeSelectChild?.Invoke();

        SelectedChild = obj;
        UpdateChildSelect_WithoutInvoke();

        if (!suppressInvokes) afterSelectChild?.Invoke();
    }

    //by Transform
    public void Select_Child(Transform obj)
    {
        if (obj.parent != transform)
            throw new ArgumentException($"Cannot select object \"{obj.name}\" beacuse it is not a child of object \"{name}\"");

        SetSelect(obj);
    }
    //by Index
    public void Select_Child(int index) => SetSelect(transform.GetChild(index));

    //First
    public void Select_FirstChild() => Select_Child(0);
    //Last
    public void Select_LastChild() => Select_Child(transform.childCount - 1);

    //Add to index
    public void Select_ChildAddToIndex_Wrap(int toAdd) => Select_Child(WrapIndex(SelectedChildIndex + toAdd));
    public void Select_ChildAddToIndex_Clamp(int toAdd) => Select_Child(ClampIndex(SelectedChildIndex + toAdd));
    //Next
    public void Select_NextChild_Wrap() => Select_ChildAddToIndex_Wrap(1);
    public void Select_NextChild_Clamp() => Select_ChildAddToIndex_Clamp(1);
    //Previous
    public void Select_PreviousChild_Wrap() => Select_ChildAddToIndex_Wrap(-1);
    public void Select_PreviousChild_Clamp() => Select_ChildAddToIndex_Clamp(-1);

    //Random
    public void Select_RandomChild() => Select_Child(Random.Range(0, transform.childCount - 1));

    //-------------------------- [ Private helper functions ] ----------------------------------------------

    private int ClampIndex(int index) => Mathf.Clamp(index, 0, transform.childCount - 1);
    private int WrapIndex(int index) => Mathf.Abs(index % transform.childCount);
}
