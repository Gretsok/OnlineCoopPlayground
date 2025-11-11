using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration.Skills
{
    public class TestSkill : ASkill
    {
        protected override void HandleSkillTriggered_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ServerCalled(a_referencesHolderForSkills);
            Debug.Log($"[SERVER] {(a_referencesHolderForSkills as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject.name} tested a skill.");
            StopSkill_ForServer();
        }

        protected override void HandleSkillTriggered_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ClientsCalled(a_referencesHolderForSkills);
            Debug.Log($"[CLIENT] {(a_referencesHolderForSkills as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject.name} tested a skill.");
        }

        protected override void HandleSkillStopped_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillStopped_ServerCalled(a_referencesHolderForSkills);
            Debug.Log($"[SERVER] {(a_referencesHolderForSkills as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject.name} stopped a skill test.");
        }

        protected override void HandleSkillStopped_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillStopped_ClientsCalled(a_referencesHolderForSkills);
            Debug.Log($"[CLIENT] {(a_referencesHolderForSkills as CharacterReferencesHolderForSkills).PlayerCharacter.gameObject.name} stopped a skill test.");
        }
    }
}
