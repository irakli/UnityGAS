using System;
using GameplayAbilitySystem.AbilitySystem.Authoring.ModifierMagnitude;
using GameplayAbilitySystem.AttributeSystem.Authoring;
using UnityEngine;
using Attribute = GameplayAbilitySystem.AttributeSystem.Authoring.Attribute;

namespace GameplayAbilitySystem.AbilitySystem
{
    [Serializable]
    public struct GameplayEffectModifier
    {
        [field: SerializeField] public Attribute Attribute { get; set; }
        [field: SerializeField] public AttributeModifier ModifierOperator { get; set; }
        [field: SerializeField] public ModifierMagnitude ModifierMagnitude { get; set; }
        [field: SerializeField] public float Multiplier { get; set; }
    }

    public enum AttributeModifier
    {
        Add,
        Multiply,
        Override
    }
}