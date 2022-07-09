using System.Collections;
using Cysharp.Threading.Tasks;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.GameplayTags.Authoring;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring
{
    public struct AbilityCooldownTime
    {
        public float TimeRemaining;
        public float TotalDuration;
    }

    public abstract class AbilitySpec
    {
        /// <summary>
        /// The ability this AbilitySpec is linked to
        /// </summary>
        public Ability Ability { get; set; }

        /// <summary>
        /// The owner of the AbilitySpec - usually the source
        /// </summary>
        protected AbilitySystemCharacter Owner { get; set; }

        /// <summary>
        /// Ability level
        /// </summary>
        public float Level { get; set; }

        /// <summary>
        /// Is this AbilitySpec currently active?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Default constructor.  Initialises the AbilitySpec from the AbstractAbilityScriptableObject
        /// </summary>
        /// <param name="ability">Ability</param>
        /// <param name="owner">Owner - usually the character activating the ability</param>
        public AbilitySpec(Ability ability, AbilitySystemCharacter owner)
        {
            Ability = ability;
            Owner = owner;
        }

        /// <summary>
        /// Try activating the ability.
        /// </summary>
        /// <returns></returns>
        public async UniTask TryStart()
        {
            if (!CanActivateAbility()) return;

            IsActive = true;
            await PreStart();
            await Start();
            End();
        }

        /// <summary>
        /// Checks if this ability can be activated
        /// </summary>
        /// <returns></returns>
        public bool CanActivateAbility()
        {
            return !IsActive
                   && SatisfiesGameplayTags()
                   && SatisfiesCost()
                   && SatisfiesCooldown().TimeRemaining <= 0;
        }

        /// <summary>
        /// Cancels the ability, if it is active
        /// </summary>
        public abstract void CancelAbility();

        /// <summary>
        /// Checks if Gameplay Tag requirements allow activating this ability
        /// </summary>
        /// <returns></returns>
        public abstract bool SatisfiesGameplayTags();

        /// <summary>
        /// Check if this ability is on cooldown
        /// </summary>
        /// <returns></returns>
        public AbilityCooldownTime SatisfiesCooldown()
        {
            float maxDuration = 0;
            if (Ability.cooldown == null) return new AbilityCooldownTime();
            var cooldownTags = Ability.cooldown.Tags.GrantedTags;

            var longestCooldown = 0f;

            // Check if the cooldown tag is granted to the player, and if so, capture the remaining duration for that tag
            foreach (var container in Owner.AppliedGameplayEffects)
            {
                var grantedTags = container.Spec.GameplayEffect.Tags
                    .GrantedTags;
                foreach (var grantedTag in grantedTags)
                {
                    foreach (var cooldownTag in cooldownTags)
                    {
                        if (grantedTag == cooldownTag)
                        {
                            if (container.Spec.GameplayEffect.DefinitionContainer
                                    .DurationPolicy == DurationPolicy.Infinite)
                                return new AbilityCooldownTime()
                                {
                                    TimeRemaining = float.MaxValue,
                                    TotalDuration = 0
                                };

                            var durationRemaining = container.Spec.DurationRemaining;

                            if (durationRemaining > longestCooldown)
                            {
                                longestCooldown = durationRemaining;
                                maxDuration = container.Spec.TotalDuration;
                            }
                        }
                    }
                }
            }

            Debug.Log(longestCooldown);
            return new AbilityCooldownTime()
            {
                TimeRemaining = longestCooldown,
                TotalDuration = maxDuration
            };
        }

        /// <summary>
        /// Method to activate before activating this ability.  This method is run after activation checks.
        /// </summary>
        protected abstract UniTask PreStart();

        /// <summary>
        /// The logic that dictates what the ability does.  Targetting logic should be placed here.
        /// Gameplay Effects are applied in this method.
        /// </summary>
        /// <returns></returns>
        protected abstract UniTask Start();

        /// <summary>
        /// Method to run once the ability ends
        /// </summary>
        public void End()
        {
            IsActive = false;
        }

        /// <summary>
        /// Checks whether the activating character has enough resources to activate this ability
        /// </summary>
        /// <returns></returns>
        public bool SatisfiesCost()
        {
            if (Ability.cost == null) return true;
            
            var spec = Owner.MakeOutgoingSpec(Ability.cost, Level);
            // If this isn't an instant cost, then assume it passes cooldown check
            if (spec.GameplayEffect.DefinitionContainer.DurationPolicy != DurationPolicy.Instant) return true;

            foreach (var modifier in spec.GameplayEffect.DefinitionContainer.Modifiers)
            {
                // Only worry about additive.  Anything else passes.
                if (modifier.ModifierOperator != AttributeModifier.Add) continue;
                var costValue = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier)
                    .GetValueOrDefault();

                Owner.AttributeSystem.GetAttributeValue(modifier.Attribute, out var attributeValue);

                // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
                if (attributeValue.CurrentValue + costValue < 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if an Ability System Character has all the listed tags
        /// </summary>
        /// <param name="character">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has all tags</returns>
        protected bool HasAllTags(AbilitySystemCharacter character, GameplayTag[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!character) return true;

            foreach (var abilityTag in tags)
            {
                var requirementPassed = false;
                foreach (var container in character.AppliedGameplayEffects)
                {
                    var ascGrantedTags = container.Spec
                        .GameplayEffect
                        .Tags.GrantedTags;
                    foreach (var tag in ascGrantedTags)
                    {
                        if (tag == abilityTag)
                        {
                            requirementPassed = true;
                        }
                    }
                }

                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if an Ability System Character has none of the listed tags
        /// </summary>
        /// <param name="character">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has none of the tags</returns>
        protected bool HasNoneTags(AbilitySystemCharacter character, GameplayTag[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!character) return true;

            foreach (var abilityTag in tags)
            {
                var requirementPassed = true;
                foreach (var container in character.AppliedGameplayEffects)
                {
                    var ascGrantedTags = container.Spec.GameplayEffect
                        .Tags.GrantedTags;
                    foreach (var tag in ascGrantedTags)
                    {
                        if (tag == abilityTag)
                        {
                            requirementPassed = false;
                        }
                    }
                }

                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }

            return true;
        }
    }
}