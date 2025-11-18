using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration.Skills.TestSkill
{
    public class TestSkill : ASkill
    {
        protected override void HandleSkillTriggered_ServerCalled(MonoBehaviour a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ServerCalled(a_referencesHolderForSkills);
            Debug.Log($"[SERVER] {a_referencesHolderForSkills.gameObject.name} tested a skill.");
            StopSkill_ForServer();
        }

        protected override void HandleSkillTriggered_ClientsCalled(MonoBehaviour a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ClientsCalled(a_referencesHolderForSkills);
            Debug.Log($"[CLIENT] {a_referencesHolderForSkills.gameObject.name} tested a skill.");
        }

        protected override void HandleSkillStopped_ServerCalled(MonoBehaviour a_referencesHolderForSkills)
        {
            base.HandleSkillStopped_ServerCalled(a_referencesHolderForSkills);
            Debug.Log($"[SERVER] {a_referencesHolderForSkills.gameObject.name} stopped a skill test.");
        }

        protected override void HandleSkillStopped_ClientsCalled(MonoBehaviour a_referencesHolderForSkills)
        {
            base.HandleSkillStopped_ClientsCalled(a_referencesHolderForSkills);
            Debug.Log($"[CLIENT] {a_referencesHolderForSkills.gameObject.name} stopped a skill test.");
        }
    }
}
