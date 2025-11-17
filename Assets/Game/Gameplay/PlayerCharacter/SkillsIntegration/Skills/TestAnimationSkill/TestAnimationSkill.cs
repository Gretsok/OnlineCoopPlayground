using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.PlayerCharacter.Movement;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration.Skills.TestAnimationSkill
{
    public class TestAnimationSkill : ASkill,
        IPlayerCharacterMovementControllerBlocker
    {
        [SerializeField]
        private float m_animationDuration = 2f;

        protected override void HandleSkillTriggered_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ServerCalled(a_referencesHolderForSkills);
            Invoke(nameof(StopSkill_ForServer), m_animationDuration);
            (a_referencesHolderForSkills as IPlayerCharacterMovementControllerHolder).MovementController.Blackboard.AddBlocker_ForServer(this);
        }

        protected override void HandleSkillTriggered_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillTriggered_ClientsCalled(a_referencesHolderForSkills);
            (a_referencesHolderForSkills as IPlayerCharacterAnimationControllerHolder).PlayerCharacterAnimationController.StartPlayingDance();
        }

        protected override void HandleSkillStopped_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillStopped_ServerCalled(a_referencesHolderForSkills);
            (a_referencesHolderForSkills as IPlayerCharacterMovementControllerHolder).MovementController.Blackboard.RemoveBlocker_ForServer(this);
        }

        protected override void HandleSkillStopped_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            base.HandleSkillStopped_ClientsCalled(a_referencesHolderForSkills);
            (a_referencesHolderForSkills as IPlayerCharacterAnimationControllerHolder).PlayerCharacterAnimationController.StopPlayingDance();
        }
    }
}
