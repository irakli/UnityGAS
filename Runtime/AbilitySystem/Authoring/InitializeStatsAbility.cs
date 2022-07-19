using Cysharp.Threading.Tasks;
using GameplayAbilitySystem.AbilitySystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Stats Initializer")]
    public class InitializeStatsAbility : Ability
    {
        [field: SerializeField] public GameplayEffect[] GameplayEffect { get; set; }

        public override AbilitySpec CreateSpec(AbilitySystemCharacter owner, AbilitySystemCharacter target = null)
        {
            var spec = new InitializeStatsAbilitySpec(this, owner)
            {
                Level = owner.Level
            };
            return spec;
        }

        public class InitializeStatsAbilitySpec : AbilitySpec
        {
            public InitializeStatsAbilitySpec(Ability ability, AbilitySystemCharacter owner) :
                base(ability, owner)
            {
            }

            public override void Cancel()
            {
            }

            public override bool SatisfiesGameplayTags()
            {
                return HasAllTags(Owner, Ability.abilityTags.OwnerTags.RequireTags)
                       && HasNoneTags(Owner, Ability.abilityTags.OwnerTags.IgnoreTags);
            }

            protected override UniTask PreStart()
            {
                return default;
            }

            protected override UniTask<bool> Start()
            {
                // Apply cost and cooldown (if any)
                if (Ability.cooldown)
                {
                    var cooldownSpec = Owner.MakeOutgoingSpec(Ability.cooldown);
                    Owner.ApplyGameplayEffectSpecToSelf(cooldownSpec);
                }

                if (Ability.cost)
                {
                    var costSpec = Owner.MakeOutgoingSpec(Ability.cost);
                    Owner.ApplyGameplayEffectSpecToSelf(costSpec);
                }

                var ability =
                    Ability as InitializeStatsAbility;
                Owner.AttributeSystem.UpdateAttributeCurrentValues();

                if (ability != null)
                {
                    foreach (var effect in ability.GameplayEffect)
                    {
                        var effectSpec = Owner.MakeOutgoingSpec(effect);
                        Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                        Owner.AttributeSystem.UpdateAttributeCurrentValues();
                    }
                }

                return default;
            }
        }
    }
}