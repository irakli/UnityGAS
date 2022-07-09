using System;
using System.Collections.Generic;
using GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Authoring
{
    public abstract class PreAttributeChangeEventArgs : EventArgs
    {
        public AttributeSystemComponent AttributeSystem { get; set; }
        public float Value { get; set; }
    }

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

        public event EventHandler PreAttributeChange;

        public void OnPreAttributeChange(object sender, PreAttributeChangeEventArgs e)
        {
            var handler = PreAttributeChange;
            PreAttributeChange?.Invoke(sender, e);
        }

        public AttributeValue CalculateInitialValue(AttributeValue attributeValue,
            List<AttributeValue> otherAttributeValues)
        {
            return attributeValue;
        }

        public virtual AttributeValue CalculateCurrentAttributeValue(AttributeValue attributeValue,
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