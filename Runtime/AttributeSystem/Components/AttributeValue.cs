using System;
using GameplayAbilitySystem.AttributeSystem.Authoring;
using UnityEngine;
using Attribute = GameplayAbilitySystem.AttributeSystem.Authoring.Attribute;

namespace GameplayAbilitySystem.AttributeSystem.Components
{
    [Serializable]
    public struct AttributeValue
    {
        [field: SerializeField] public Attribute Attribute { get; set; }
        [field: SerializeField] public float BaseValue { get; set; }
        [field: SerializeField] public float CurrentValue { get; set; }
        [field: SerializeField] public AttributeModifier Modifier { get; set; }
    }

    [Serializable]
    public struct AttributeModifier
    {
        [field: SerializeField] public float Add { get; set; }
        [field: SerializeField] public float Multiply { get; set; }
        [field: SerializeField] public float Override { get; set; }

        public AttributeModifier Combine(AttributeModifier other)
        {
            other.Add += Add;
            other.Multiply += Multiply;
            other.Override = Override;
            
            return other;
        }
    }
}