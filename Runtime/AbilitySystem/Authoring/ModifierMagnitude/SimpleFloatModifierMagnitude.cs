using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring.ModifierMagnitude
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect/Modifier Magnitude/Simple Float")]
    public class SimpleFloatModifierMagnitude : ModifierMagnitude
    {
        [SerializeField] private AnimationCurve scalingFunction;

        public override void Initialize(GameplayEffectSpec spec)
        {
        }

        public override float? CalculateMagnitude(GameplayEffectSpec spec)
        {
            return scalingFunction.Evaluate(spec.Level);
        }
    }
}