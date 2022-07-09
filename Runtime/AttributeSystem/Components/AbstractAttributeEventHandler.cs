using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Components
{
    public abstract class AbstractAttributeEventHandler : ScriptableObject
    {
        public abstract void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues);
    }
}
