using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ErryLib.ModiferSystem.Instancers
{
    /// <summary>
    /// This allows classes to register themselves to the Modifier system, but by instancing dummy modifiers and registering them instead. 
    /// <br/> - The dummy InstancedModifier can also be passed data that is specific to that one modifier instance.
    /// <br/> - When the dummy InstancedModifier gets OnModifyValue(...) called, it passes that call to this instancer, alongside the data 
    /// that was applied to it when it was created and registered
    /// <br/> - To unregister the modifiers, take the modifier returned by GetNewRegisteredModifier(data) and call UnregisterModifier()
    /// </summary>
    public interface IModifierInstancer<TData>
    {
        /// <summary>Call this to get a new modifier containing the data passed in.
        /// <br/> - This modifier is preregistered to the active modifiers, and you will have to call UnregisterModifier on it
        /// to unregister the modifier
        /// </summary>
        public virtual Modifier GetNewRegisteredModifier(TData data)
        {
            Modifier modifier = new InstancedModifier<TData>(this, data);
            modifier.RegisterModifier();
            return modifier;
        }

        /// <summary>Called when a modifier created from this instancer is successfully ADDED from active modifiers</summary>
        protected virtual void OnInstanceRegistered(InstancedModifier<TData> instancedModifier) { }
        /// <summary>Called when a modifier created from this instancer is successfully REMOVED from active modifiers</summary>
        protected virtual void OnInstanceUnregistered(InstancedModifier<TData> instancedModifier) { }

        /// <summary>Called when a modifier created from this instancer is being applied to a modifiable, 
        /// and is passed here for the instancer to handle</summary>
        protected void OnInstanceModifyValue(Modifiable modifiableValue, TData data);


        //-------------------------------------------------------------------------------------------------------------
        // Below methods are called from InstancedModifier itself to pass the methods to the Instancer
        //-------------------------------------------------------------------------------------------------------------

        internal void Invoke_OnInstanceRegistered(InstancedModifier<TData> data) => OnInstanceRegistered(data);
        internal void Invoke_OnInstanceUnregistered(InstancedModifier<TData> data) => OnInstanceUnregistered(data);
        internal void Invoke_OnInstanceModifyValue(Modifiable modifiableValue, TData data) =>
            OnInstanceModifyValue(modifiableValue, data);
    }


    /// <summary>
    /// A modifier generated through IModifierInstancer
    /// <br/> - Contains local data for this specific modifier instance that is passed back to the 
    /// <br/> - To unregister the modifier, call UnregisterModifier()
    /// <br/> - You can also call RegisterModifier() to re-register the modifier again if you need to
    /// </summary>
    public class InstancedModifier<TData> : Modifier 
    {
        public IModifierInstancer<TData> Instancer { get; private set; }
        public TData ModifierData;
        internal InstancedModifier(IModifierInstancer<TData> from, TData data)
        {
            this.Instancer = from;
            this.ModifierData = data;
        }


        /// <summary>Called when this modifier is successfully registered as active modifier</summary>
        protected override void OnRegisterModifier() =>
            Instancer.Invoke_OnInstanceRegistered(this);

        /// <summary>Called when this modifier is successfully removed from active modifiers</summary>
        protected override void OnUnregisterModifier() =>
            Instancer.Invoke_OnInstanceUnregistered(this);

        public override void ModifyValue(Modifiable modifiableValue) =>
            Instancer.Invoke_OnInstanceModifyValue(modifiableValue, ModifierData);
    }
}