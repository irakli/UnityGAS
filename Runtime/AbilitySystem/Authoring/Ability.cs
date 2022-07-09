using GameplayAbilitySystem.AbilitySystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring
{
    public abstract class Ability : ScriptableObject
    {
        /// <summary>
        /// Name of this ability
        /// </summary>
        [SerializeField] private string abilityName;

        /// <summary>
        /// Tags for this ability
        /// </summary>
        [SerializeField] public AbilityTags abilityTags;

        /// <summary>
        /// The GameplayEffect that defines the cost associated with activating the ability
        /// </summary>
        /// <returns></returns>
        [SerializeField] public GameplayEffect cost;

        /// <summary>
        /// The GameplayEffect that defines the cooldown associated with this ability
        /// </summary>
        /// <returns></returns>
        [SerializeField] public GameplayEffect cooldown;

        /// <summary>
        /// Creates the Ability Spec (the instantiation of the ability)
        /// </summary>
        /// <param name="owner">Usually the character casting this ability</param>
        /// <returns>Ability Spec</returns>
        public abstract AbilitySpec CreateSpec(AbilitySystemCharacter owner);
    }
}