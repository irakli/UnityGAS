using System.Collections;
using Cysharp.Threading.Tasks;
using GameplayAbilitySystem.AbilitySystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.Authoring
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Simple Ability")]
    public class SimpleAbility : Ability
    {
        /// <summary>
        /// Gameplay Effect to apply
        /// </summary>
        public GameplayEffect GameplayEffect;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new SimpleAbilitySpec(this, owner)
            {
                Level = owner.Level
            };
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// </summary>
        public class SimpleAbilitySpec : AbilitySpec
        {
            public SimpleAbilitySpec(Ability ability, AbilitySystemCharacter owner) : base(ability, owner)
            {
            }

            /// <summary>
            /// What to do when the ability is cancelled.  We don't care about there for this example.
            /// </summary>
            public override void CancelAbility() { }

            /// <summary>
            /// What happens when we activate the ability.
            /// 
            /// In this example, we apply the cost and cooldown, and then we apply the main
            /// gameplay effect
            /// </summary>
            /// <returns></returns>
            protected override UniTask Start()
            {
                // Apply cost and cooldown
                var cooldownSpec = Owner.MakeOutgoingSpec(Ability.cooldown);
                var costSpec = Owner.MakeOutgoingSpec(Ability.cost);
                Owner.ApplyGameplayEffectSpecToSelf(cooldownSpec);
                Owner.ApplyGameplayEffectSpecToSelf(costSpec);
                
                // Apply primary effect
                var effectSpec = Owner.MakeOutgoingSpec((Ability as SimpleAbility)?.GameplayEffect);
                Owner.ApplyGameplayEffectSpecToSelf(effectSpec);

                return default;
            }

            /// <summary>
            /// Checks to make sure Gameplay Tags checks are met. 
            /// 
            /// Since the target is also the character activating the ability,
            /// we can just use Owner for all of them.
            /// </summary>
            /// <returns></returns>
            public override bool SatisfiesGameplayTags()
            {
                return HasAllTags(Owner, Ability.abilityTags.OwnerTags.RequireTags)
                        && HasNoneTags(Owner, Ability.abilityTags.OwnerTags.IgnoreTags)
                        && HasAllTags(Owner, Ability.abilityTags.SourceTags.RequireTags)
                        && HasNoneTags(Owner, Ability.abilityTags.SourceTags.IgnoreTags)
                        && HasAllTags(Owner, Ability.abilityTags.TargetTags.RequireTags)
                        && HasNoneTags(Owner, Ability.abilityTags.TargetTags.IgnoreTags);
            }

            /// <summary>
            /// Logic to execute before activating the ability.  We don't need to do anything here
            /// for this example.
            /// </summary>
            /// <returns></returns>

            protected override UniTask PreStart()
            {
                return default;
            }
        }
    }

}