using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring.ModifierMagnitude
{
    public abstract class ModifierMagnitude : ScriptableObject
    {
        /// <summary>
        /// Function called when the spec is first initialized (e.g. by the Instigator/Source Ability System)
        /// </summary>
        /// <param name="spec">Gameplay Effect Spec</param>
        public abstract void Initialize(GameplayEffectSpec spec);

        /// <summary>
        /// Function called when the magnitude is calculated, usually after the target has been assigned
        /// </summary>
        /// <param name="spec">Gameplay Effect Spec</param>
        /// <returns></returns>
        public abstract float? CalculateMagnitude(GameplayEffectSpec spec);
    }
}