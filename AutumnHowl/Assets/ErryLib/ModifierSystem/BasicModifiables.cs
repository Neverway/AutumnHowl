using System;
using UnityEngine;

[Serializable]
public abstract class NumberModifier<TTarget> : Modifier where TTarget : INumberModifiable
{
    public enum ModifierType { Add, Multiply }
    public ModifierType modifierType;
    public float value;

    public override void ModifyValue(Modifiable modifiableValue)
    {
        if (modifiableValue is not TTarget number)
            return;

        if (modifierType == ModifierType.Add)
            number.OnModify_AddNumber(value);

        if (modifierType == ModifierType.Multiply)
            number.OnModify_MultiplyNumber(value);
    }
}
public interface INumberModifiable : Modifiable
{
    public void OnModify_AddNumber(float toAdd);
    public void OnModify_MultiplyNumber(float toMultiply);
}
[Serializable]
public abstract class ModifiableInt : Modifiable<int>, INumberModifiable
{
    [SerializeField] private int startValue;
    [HideInInspector] public float multiplier;
    [HideInInspector] public float addedValue;

    public ModifiableInt(int startValue) { this.startValue = startValue; }

    protected override void SetInitialValue()
    {
        addedValue = 0f;
        multiplier = 1f;
    }
    protected override int GetFinalizedValue() =>
        Mathf.RoundToInt((startValue + addedValue) * multiplier);
    public void OnModify_AddNumber(float toAdd) => addedValue += toAdd;
    public void OnModify_MultiplyNumber(float toMultiply) => multiplier *= toMultiply;
}

[Serializable]
public abstract class ModifiableFloat : Modifiable<float>, INumberModifiable
{
    [SerializeField] private float startValue;
    [HideInInspector] public float multiplier;
    [HideInInspector] public float addedValue;

    public ModifiableFloat(float startValue) { this.startValue = startValue; }
    protected override void SetInitialValue()
    {
        addedValue = 0f;
        multiplier = 1f;
    }

    protected override float GetFinalizedValue() => (startValue + addedValue) * multiplier;
    public void OnModify_AddNumber(float toAdd) => addedValue += toAdd;
    public void OnModify_MultiplyNumber(float toMultiply) => multiplier *= toMultiply;
}
