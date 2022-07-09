using System;
using GameplayAbilitySystem.GameplayTags.Authoring;

namespace GameplayAbilitySystem.AbilitySystem
{
    [Serializable]
    public struct GameplayTagRequireIgnoreContainer
    {
        /// <summary>
        /// All of these tags must be present
        /// </summary>
        public GameplayTag[] RequireTags;

        /// <summary>
        /// None of these tags can be present
        /// </summary>
        public GameplayTag[] IgnoreTags;
    }

}
