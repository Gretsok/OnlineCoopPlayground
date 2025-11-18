using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration.Skills.TestSkillWithEffect
{
    public class TestSkillWithEffect : ASkill
    {
        [SerializeField]
        private EffectOfTestSkillWithEffect m_effectPrefab;

        protected override void HandleSkillTriggered_ServerCalled(MonoBehaviour a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ServerCalled(a_referencesHolderForSkills);
            var effect = (a_referencesHolderForSkills as IEffectsControllerHolder).EffectsController.AddTimedEffect_ForServer(m_effectPrefab);
            StopSkill_ForServer();
        }
    }
}
