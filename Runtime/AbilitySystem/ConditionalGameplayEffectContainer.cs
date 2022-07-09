using System;
using GameplayAbilitySystem.AbilitySystem.Authoring;
using GameplayAbilitySystem.GameplayTags.Authoring;

namespace GameplayAbilitySystem.AbilitySystem
{
    [Serializable]
    public struct ConditionalGameplayEffectContainer
    {
        public GameplayEffect GameplayEffect;
        public GameplayTag[] RequiredSourceTags;
    }

}
