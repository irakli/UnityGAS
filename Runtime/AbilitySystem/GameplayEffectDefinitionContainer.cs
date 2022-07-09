using System;
using GameplayAbilitySystem.AbilitySystem.Authoring.ModifierMagnitude;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem
{
    [Serializable]
    public struct GameplayEffectDefinitionContainer
    {
        /// <summary>
        /// The duration of this Gameplay Effect.
        /// Instant Gameplay Effects are applied immediately and then removed,
        /// and Infinite and Has Duration are persistent and remain applied
        /// </summary>
        [field: SerializeField] public DurationPolicy DurationPolicy { get; set; }

        [field: SerializeField] public ModifierMagnitude DurationModifier { get; set; }

        /// <summary>
        /// The duration of this Gameplay Effect, if the Gameplay Effect has a finite duration
        /// </summary>
        [field: SerializeField] public float DurationMultiplier { get; set; }

        /// <summary>
        /// The attribute modifications that this Gameplay Effect provides
        /// </summary>
        [field: SerializeField] public GameplayEffectModifier[] Modifiers { get; set; }

        /// <summary>
        /// Other Gameplay Effect to apply to the source ability system,
        /// based on presence of tags on source
        /// </summary>
        [field: SerializeField] public ConditionalGameplayEffectContainer[] ConditionalGameplayEffects { get; set; }
    }
}