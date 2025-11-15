using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.PlayerCharacter.Movement;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration
{
    public class CharacterReferencesHolderForSkills : AReferencesHolderForSkills, 
        IPlayerCharacterMovementControllerHolder, 
        IRigidbodyHolder,
        ISkillCasterHolder,
        ISkillsInventoryHolder, 
        IEffectsControllerHolder,
        IPlayerCharacterAnimationControllerHolder,
        IIsGroundedControllerHolder
    {
        [field: SerializeField]
        public PlayerCharacter PlayerCharacter { get; private set; }

        public PlayerCharacterMovementController MovementController => PlayerCharacter.MovementController;
        public Rigidbody Rigidbody => PlayerCharacter.Rigidbody;
        public SkillCaster SkillCaster => PlayerCharacter.SkillCaster;
        public SkillsInventory SkillsInventory => PlayerCharacter.SkillsInventory;
        public EffectsController EffectsController => PlayerCharacter.EffectsController;
        public PlayerCharacterAnimationController PlayerCharacterAnimationController =>
            PlayerCharacter.PlayerCharacterAnimationController;

        public IsGroundedController IsGroundedController => PlayerCharacter.IsGroundedController;
    }
}
