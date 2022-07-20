using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Attribute = GameplayAbilitySystem.AttributeSystem.Authoring.Attribute;

namespace GameplayAbilitySystem.AttributeSystem.Components
{
    /// <summary>
    /// Manages the attributes for a game character
    /// </summary>
    public class AttributeSystem : MonoBehaviour
    {
        public class AttributeChangeEventArgs : EventArgs
        {
            public Attribute Attribute { get; set; }
            public float CurrentValue { get; set; }
            public float PreviousValue { get; set; }
        }

        public event EventHandler<AttributeChangeEventArgs> AttributeChanged;

        [SerializeField] private AbstractAttributeEventHandler[] attributeSystemEvents;

        /// <summary>
        /// Attribute sets assigned to the game character
        /// </summary>
        [SerializeField] private List<Attribute> attributes;

        [SerializeField] private List<AttributeValue> attributeValues;

        private bool _isAttributeDictStale;
        public Dictionary<Attribute, int> AttributeIndexCache { get; } = new();

        /// <summary>
        /// Marks attribute cache dirty, so it can be recreated next time it is required
        /// </summary>
        public void MarkAttributesDirty()
        {
            _isAttributeDictStale = true;
        }

        /// <summary>
        /// Gets the value of an attribute.  Note that the returned value is a copy of the struct, so modifying it
        /// does not modify the original attribute
        /// </summary>
        /// <param name="attribute">Attribute to get value for</param>
        /// <param name="value">Returned attribute</param>
        /// <returns>True if attribute was found, false otherwise.</returns>
        public bool GetAttributeValue(Attribute attribute, out AttributeValue value)
        {
            // If dictionary is stale, rebuild it
            var attributeCache = GetAttributeCache();

            // We use a cache to store the index of the attribute in the list, so we don't
            // have to iterate through it every time
            if (attributeCache.TryGetValue(attribute, out var index))
            {
                value = attributeValues[index];
                return true;
            }

            // No matching attribute found
            value = new AttributeValue();
            return false;
        }

        public void SetAttributeBaseValue(Attribute attribute, float value)
        {
            // If dictionary is stale, rebuild it
            var attributeCache = GetAttributeCache();
            if (!attributeCache.TryGetValue(attribute, out var index)) return;

            var attributeValue = attributeValues[index];
            attributeValue.BaseValue = value;
            attributeValues[index] = attributeValue;
        }

        /// <summary>
        /// Sets value of an attribute.  Note that the out value is a copy of the struct, so modifying it
        /// does not modify the original attribute
        /// </summary>
        /// <param name="attribute">Attribute to set</param>
        /// <param name="modifier">How to modify the attribute</param>
        /// <param name="value">Copy of newly modified attribute</param>
        /// <returns>True, if attribute was found.</returns>
        public bool UpdateAttributeModifiers(Attribute attribute, AttributeModifier modifier, out AttributeValue value)
        {
            // If dictionary is stale, rebuild it
            var attributeCache = GetAttributeCache();

            // We use a cache to store the index of the attribute in the list, so we don't
            // have to iterate through it every time
            if (attributeCache.TryGetValue(attribute, out var index))
            {
                // Get a copy of the attribute value struct
                value = attributeValues[index];
                value.Modifier = value.Modifier.Combine(modifier);

                // Structs are copied by value, so the modified attribute needs to be reassigned to the array
                attributeValues[index] = value;
                return true;
            }

            // No matching attribute found
            value = new AttributeValue();
            return false;
        }

        /// <summary>
        /// Add attributes to this attribute system.  Duplicates are ignored.
        /// </summary>
        /// <param name="attributes">Attributes to add</param>
        public void AddAttributes(params Attribute[] attributes)
        {
            // If this attribute already exists, we don't need to add it.  For that, we need to make sure the cache is up to date.
            var attributeCache = GetAttributeCache();

            for (var i = 0; i < attributes.Length; i++)
            {
                if (attributeCache.ContainsKey(attributes[i]))
                {
                    continue;
                }

                this.attributes.Add(attributes[i]);
                attributeCache.Add(attributes[i], this.attributes.Count - 1);
            }
        }

        /// <summary>
        /// Remove attributes from this attribute system.
        /// </summary>
        /// <param name="attributes">Attributes to remove</param>
        public void RemoveAttributes(params Attribute[] attributes)
        {
            for (var i = 0; i < attributes.Length; i++)
            {
                this.attributes.Remove(attributes[i]);
            }

            // Update attribute cache
            GetAttributeCache();
        }

        public void ResetAll()
        {
            for (var i = 0; i < attributeValues.Count; i++)
            {
                var defaultAttribute = new AttributeValue
                {
                    Attribute = attributeValues[i].Attribute
                };
                attributeValues[i] = defaultAttribute;
            }
        }

        public void ResetAttributeModifiers()
        {
            for (var i = 0; i < attributeValues.Count; i++)
            {
                var attributeValue = attributeValues[i];
                attributeValue.Modifier = default;
                attributeValues[i] = attributeValue;
            }
        }

        private void InitializeAttributeValues()
        {
            foreach (var attribute in attributes.Where(attribute => attributeValues.All(a => a.Attribute != attribute)))
            {
                attributeValues.Add(new AttributeValue
                    {
                        Attribute = attribute,
                        Modifier = new AttributeModifier
                        {
                            Add = 0f,
                            Multiply = 0f,
                            Override = 0f
                        }
                    }
                );
            }
        }

        private readonly List<AttributeValue> _previousAttributeValues = new();

        public void UpdateAttributeCurrentValues()
        {
            _previousAttributeValues.Clear();
            for (var i = 0; i < attributeValues.Count; i++)
            {
                var attribute = attributeValues[i];
                _previousAttributeValues.Add(attribute);
                attributeValues[i] =
                    attribute.Attribute.CalculateCurrentValue(attribute, attributeValues);

                if (attributeValues[i].CurrentValue != _previousAttributeValues[i].CurrentValue)
                {
                    AttributeChanged?.Invoke(this, new AttributeChangeEventArgs()
                    {
                        Attribute = attributes[i],
                        CurrentValue = attributeValues[i].CurrentValue,
                        PreviousValue = _previousAttributeValues[i].CurrentValue
                    });
                }
            }

            for (var i = 0; i < attributeSystemEvents.Length; i++)
            {
                attributeSystemEvents[i].PreAttributeChange(this, _previousAttributeValues, ref attributeValues);
            }
        }

        private Dictionary<Attribute, int> GetAttributeCache()
        {
            if (_isAttributeDictStale)
            {
                AttributeIndexCache.Clear();
                for (var i = 0; i < attributeValues.Count; i++)
                {
                    AttributeIndexCache.Add(attributeValues[i].Attribute, i);
                }

                _isAttributeDictStale = false;
            }

            return AttributeIndexCache;
        }

        private void Awake()
        {
            InitializeAttributeValues();
            MarkAttributesDirty();
            GetAttributeCache();
        }

        private void LateUpdate()
        {
            UpdateAttributeCurrentValues();
        }
    }
}