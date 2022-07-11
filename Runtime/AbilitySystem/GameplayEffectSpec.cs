using System;
using GameplayAbilitySystem.AbilitySystem.Authoring;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.AttributeSystem.Components;

namespace GameplayAbilitySystem.AbilitySystem
{
    [Serializable]
    public class GameplayEffectSpec
    {
        /// <summary>
        /// Original gameplay effect that is the base for this spec
        /// </summary>
        public GameplayEffect GameplayEffect { get; private set; }

        public float DurationRemaining { get; private set; }
        public float TotalDuration { get; private set; }
        public float TimeUntilPeriodTick { get; private set; }
        public float Level { get; private set; }
        
        public AbilitySystemCharacter Source { get; private set; }
        public AbilitySystemCharacter Target { get; private set; }
        public AttributeValue? SourceCapturedAttribute = null;
        public GameplayEffectPeriod PeriodDefinition { get; private set; }

        public static GameplayEffectSpec CreateNew(GameplayEffect gameplayEffect, AbilitySystemCharacter source,
            float level = 1)
        {
            return new GameplayEffectSpec(gameplayEffect, source, level);
        }

        private GameplayEffectSpec(GameplayEffect gameplayEffect, AbilitySystemCharacter source, float level = 1)
        {
            GameplayEffect = gameplayEffect;
            Source = source;
            for (var i = 0; i < GameplayEffect.DefinitionContainer.Modifiers.Length; i++)
            {
                GameplayEffect.DefinitionContainer.Modifiers[i].ModifierMagnitude.Initialize(this);
            }

            Level = level;
            if (GameplayEffect.DefinitionContainer.DurationModifier)
            {
                DurationRemaining =
                    GameplayEffect.DefinitionContainer.DurationModifier.CalculateMagnitude(this)
                        .GetValueOrDefault() * GameplayEffect.DefinitionContainer.DurationMultiplier;
                TotalDuration = DurationRemaining;
            }

            TimeUntilPeriodTick = GameplayEffect.Period.Period;
            // By setting the time to 0, we make sure it gets executed at first opportunity
            if (GameplayEffect.Period.ExecuteOnApplication)
            {
                TimeUntilPeriodTick = 0;
            }
        }

        public GameplayEffectSpec SetTarget(AbilitySystemCharacter target)
        {
            Target = target;
            return this;
        }

        public void SetTotalDuration(float totalDuration)
        {
            TotalDuration = totalDuration;
        }

        public GameplayEffectSpec SetDuration(float duration)
        {
            DurationRemaining = duration;
            return this;
        }

        public GameplayEffectSpec UpdateRemainingDuration(float deltaTime)
        {
            DurationRemaining -= deltaTime;
            return this;
        }

        public GameplayEffectSpec TickPeriodic(float deltaTime, out bool executePeriodicTick)
        {
            TimeUntilPeriodTick -= deltaTime;
            executePeriodicTick = false;
            if (TimeUntilPeriodTick <= 0)
            {
                TimeUntilPeriodTick = GameplayEffect.Period.Period;

                // Check to make sure period is valid, otherwise we'd just end up executing every frame
                if (GameplayEffect.Period.Period > 0)
                {
                    executePeriodicTick = true;
                }
            }

            return this;
        }

        public GameplayEffectSpec SetLevel(float level)
        {
            Level = level;
            return this;
        }
    }
}