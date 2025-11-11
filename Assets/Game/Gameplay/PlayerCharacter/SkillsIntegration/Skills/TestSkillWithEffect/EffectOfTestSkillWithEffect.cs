using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration.Skills.TestSkillWithEffect
{
    public class EffectOfTestSkillWithEffect : TimedEffect
    {
        protected override void HandleActiveEffectStarted_ServerCalled()
        {
            base.HandleActiveEffectStarted_ServerCalled();
            Debug.Log($"[SERVER] Test effect started at time {NetworkManager.ServerTime.FixedTime}.", (ReferencesHolder as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject);
        }

        protected override void HandleActiveEffectStarted_ClientsCalled()
        {
            base.HandleActiveEffectStarted_ClientsCalled();
            Debug.Log($"[CLIENT] Test effect started at time {NetworkManager.ServerTime.FixedTime}.", (ReferencesHolder as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject);
        }

        protected override void HandleActiveEffectEnded_ServerCalled()
        {
            base.HandleActiveEffectEnded_ServerCalled();
            Debug.Log($"[SERVER] Test effect ended at time {NetworkManager.ServerTime.FixedTime}.", (ReferencesHolder as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject);
        }

        protected override void HandleActiveEffectEnded_ClientsCalled()
        {
            base.HandleActiveEffectEnded_ClientsCalled();
            Debug.Log($"[CLIENT] Test effect ended at time {NetworkManager.ServerTime.FixedTime}.", (ReferencesHolder as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject);
        }
    }
}
