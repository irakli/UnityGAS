using System;
using GameplayAbilitySystem.AttributeSystem.Authoring;
using Attribute = GameplayAbilitySystem.AttributeSystem.Authoring.Attribute;

namespace GameplayAbilitySystem.AttributeSystem.Components
{
    [Serializable]
    public struct AttributeValue
    {
        public Attribute Attribute;
        public float BaseValue;
        public float CurrentValue;
        public AttributeModifier Modifier;
    }

    [Serializable]
    public struct AttributeModifier
    {
        public float Add;
        public float Multiply;
        public float Override;
        public AttributeModifier Combine(AttributeModifier other)
        {
            other.Add += Add;
            other.Multiply += Multiply;
            other.Override = Override;
            return other;
        }
    }

}
