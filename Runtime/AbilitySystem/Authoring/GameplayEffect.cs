using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect Definition")]
    public class GameplayEffect : ScriptableObject
    {
        [field: SerializeField]
        public GameplayEffectDefinitionContainer DefinitionContainer { get; set; }

        [field: SerializeField]
        public GameplayEffectTags Tags { get; set; }

        [field: SerializeField]
        public GameplayEffectPeriod Period { get; set; }
    }
}