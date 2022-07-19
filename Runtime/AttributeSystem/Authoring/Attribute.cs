using System;
using System.Collections.Generic;
using GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Authoring
{
    /// <summary>
    /// This asset defines a single player attribute
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Attribute")]
    public class Attribute : ScriptableObject
    {
        /// <summary>
        /// Friendly name of this attribute.  Used for display purposes only.
        /// </summary>
        [field: SerializeField] public string Name { get; set; }

        public AttributeValue CalculateInitialValue(AttributeValue attributeValue,
            List<AttributeValue> otherAttributeValues)
        {
            return attributeValue;
        }

        public virtual AttributeValue CalculateCurrentValue(AttributeValue attributeValue,
            List<AttributeValue> otherAttributeValues)
        {
            attributeValue.CurrentValue = (attributeValue.BaseValue + attributeValue.Modifier.Add) *
                                          (attributeValue.Modifier.Multiply + 1);

            if (attributeValue.Modifier.Override != 0)
            {
                attributeValue.CurrentValue = attributeValue.Modifier.Override;
            }

            return attributeValue;
        }
    }
}