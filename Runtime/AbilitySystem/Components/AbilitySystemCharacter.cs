using System.Collections.Generic;
using GameplayAbilitySystem.AbilitySystem.Authoring;
using GameplayAbilitySystem.AttributeSystem.Authoring;
using GameplayAbilitySystem.AttributeSystem.Components;
using GameplayAbilitySystem.GameplayTags.Authoring;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Components
{
    public class AbilitySystemCharacter : MonoBehaviour
    {
        [field: SerializeField] public AttributeSystemComponent AttributeSystem { get; set; }
        [field: SerializeField] public float Level { get; set; }

        public readonly List<GameplayEffectContainer> AppliedGameplayEffects = new();
        public readonly List<AbilitySpec> GrantedAbilities = new();

        public void GrantAbility(AbilitySpec spec)
        {
            GrantedAbilities.Add(spec);
        }

        public void RemoveAbilitiesWithTag(GameplayTag tag)
        {
            for (var i = GrantedAbilities.Count - 1; i >= 0; i--)
            {
                if (GrantedAbilities[i].Ability.abilityTags.AssetTag == tag)
                {
                    GrantedAbilities.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Applies the gameplay effect spec to self
        /// </summary>
        /// <param name="geSpec">GameplayEffectSpec to apply</param>
        public bool ApplyGameplayEffectSpecToSelf(GameplayEffectSpec geSpec)
        {
            if (geSpec == null) return true;
            bool tagRequirementsOK = SatisfiesRequirements(geSpec);

            if (tagRequirementsOK == false) return false;


            switch (geSpec.GameplayEffect.DefinitionContainer.DurationPolicy)
            {
                case DurationPolicy.HasDuration:
                case DurationPolicy.Infinite:
                    ApplyDurationalGameplayEffect(geSpec);
                    break;
                case DurationPolicy.Instant:
                    ApplyInstantGameplayEffect(geSpec);
                    return true;
            }

            return true;
        }

        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffect GameplayEffect, float? level = 1f)
        {
            level = level ?? Level;
            return GameplayEffectSpec.CreateNew(
                gameplayEffect: GameplayEffect,
                source: this,
                level: level.GetValueOrDefault(1));
        }

        private bool SatisfiesRequirements(GameplayEffectSpec spec)
        {
            // Build temporary list of all tags currently applied

            var appliedTags = new List<GameplayTag>();
            foreach (var container in AppliedGameplayEffects)
            {
                appliedTags.AddRange(container.Spec.GameplayEffect.Tags.GrantedTags);
            }

            // Every tag in the ApplicationTagRequirements.RequireTags needs to be in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.RequireTags is not present, requirement is not met
            foreach (var tag in spec.GameplayEffect.Tags.ApplicationTagRequirements.RequireTags)
            {
                if (!appliedTags.Contains(tag))
                {
                    return false;
                }
            }

            // No tag in the ApplicationTagRequirements.IgnoreTags must in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.IgnoreTags is present, requirement is not met
            foreach (var tag in spec.GameplayEffect.Tags.ApplicationTagRequirements.IgnoreTags)
            {
                if (appliedTags.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }

        private void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
        {
            for (var i = 0; i < spec.GameplayEffect.DefinitionContainer.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.DefinitionContainer.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier)
                    .GetValueOrDefault();
                var attribute = modifier.Attribute;
                AttributeSystem.GetAttributeValue(attribute, out var attributeValue);

                switch (modifier.ModifierOperator)
                {
                    case AttributeModifier.Add:
                        attributeValue.BaseValue += magnitude;
                        break;
                    case AttributeModifier.Multiply:
                        attributeValue.BaseValue *= magnitude;
                        break;
                    case AttributeModifier.Override:
                        attributeValue.BaseValue = magnitude;
                        break;
                }

                AttributeSystem.SetAttributeBaseValue(attribute, attributeValue.BaseValue);
            }
        }

        private void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
            foreach (var modifier in spec.GameplayEffect.DefinitionContainer.Modifiers)
            {
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier)
                    .GetValueOrDefault();
                var attributeModifier = new AttributeSystem.Components.AttributeModifier();
                switch (modifier.ModifierOperator)
                {
                    case AttributeModifier.Add:
                        attributeModifier.Add = magnitude;
                        break;
                    case AttributeModifier.Multiply:
                        attributeModifier.Multiply = magnitude;
                        break;
                    case AttributeModifier.Override:
                        attributeModifier.Override = magnitude;
                        break;
                }

                modifiersToApply.Add(new GameplayEffectContainer.ModifierContainer()
                    { Attribute = modifier.Attribute, Modifier = attributeModifier });
            }

            AppliedGameplayEffects.Add(new GameplayEffectContainer()
                { Spec = spec, Modifiers = modifiersToApply.ToArray() });
        }

        private void UpdateAttributeSystem()
        {
            // Set Current Value to Base Value

            foreach (var container in AppliedGameplayEffects)
            {
                var modifiers = container.Modifiers;
                foreach (var modifier in modifiers)
                {
                    AttributeSystem.UpdateAttributeModifiers(modifier.Attribute, modifier.Modifier, out _);
                }
            }
        }

        private void TickGameplayEffects()
        {
            foreach (var container in AppliedGameplayEffects)
            {
                var spec = container.Spec;

                // Can't tick instant GE
                if (spec.GameplayEffect.DefinitionContainer.DurationPolicy == DurationPolicy.Instant) continue;

                // Update time remaining.  Strictly, it's only really valid for durational GE, but calculating for infinite GE isn't harmful
                spec.UpdateRemainingDuration(Time.deltaTime);

                // Tick the periodic component
                spec.TickPeriodic(Time.deltaTime, out var executePeriodicTick);
                if (executePeriodicTick)
                {
                    ApplyInstantGameplayEffect(spec);
                }
            }
        }

        private void CleanGameplayEffects()
        {
            AppliedGameplayEffects.RemoveAll(x =>
                x.Spec.GameplayEffect.DefinitionContainer.DurationPolicy == DurationPolicy.HasDuration &&
                x.Spec.DurationRemaining <= 0);
        }

        private void Update()
        {
            // Reset all attributes to 0
            AttributeSystem.ResetAttributeModifiers();
            UpdateAttributeSystem();

            TickGameplayEffects();
            CleanGameplayEffects();
        }
    }


    public class GameplayEffectContainer
    {
        public GameplayEffectSpec Spec { get; set; }
        public ModifierContainer[] Modifiers { get; set; }

        public class ModifierContainer
        {
            public Attribute Attribute { get; set; }
            public AttributeSystem.Components.AttributeModifier Modifier { get; set; }
        }
    }
}