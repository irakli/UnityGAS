using GameplayAbilitySystem.AttributeSystem.Authoring;
using GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring.ModifierMagnitude
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect/Modifier Magnitude/Attribute Backed")]
    public class AttributeBackedModifierMagnitude : ModifierMagnitude
    {


        [SerializeField]
        private AnimationCurve ScalingFunction;

        [SerializeField]
        private Attribute CaptureAttributeWhich;

        [SerializeField]
        private ECaptureAttributeFrom CaptureAttributeFrom;

        [SerializeField]
        private ECaptureAttributeWhen CaptureAttributeWhen;


        public override void Initialize(GameplayEffectSpec spec)
        {
            spec.Source.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var sourceAttributeValue);
            spec.SourceCapturedAttribute = sourceAttributeValue;
        }

        public override float? CalculateMagnitude(GameplayEffectSpec spec)
        {

            return ScalingFunction.Evaluate(GetCapturedAttribute(spec).GetValueOrDefault().CurrentValue);
        }

        private AttributeValue? GetCapturedAttribute(GameplayEffectSpec spec)
        {
            if (CaptureAttributeWhen == ECaptureAttributeWhen.OnApplication && CaptureAttributeFrom == ECaptureAttributeFrom.Source)
            {
                return spec.SourceCapturedAttribute;
            }

            switch (CaptureAttributeFrom)
            {
                case ECaptureAttributeFrom.Source:
                    spec.Source.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var sourceAttributeValue);
                    return sourceAttributeValue;
                case ECaptureAttributeFrom.Target:
                    spec.Target.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var targetAttributeValue);
                    return targetAttributeValue;
                default:
                    return null;
            }
        }
    }
}
