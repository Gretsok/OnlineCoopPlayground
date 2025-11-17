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
        IIsGroundedControllerHolder
    {
        [field: SerializeField]
        public DefaultPlayerMotor DefaultPlayerMotor { get; private set; }

        public PlayerCharacterMovementController MovementController => DefaultPlayerMotor.MovementController;
        public Rigidbody Rigidbody => DefaultPlayerMotor.Rigidbody;
        public SkillCaster SkillCaster => DefaultPlayerMotor.SkillCaster;
        public SkillsInventory SkillsInventory => DefaultPlayerMotor.SkillsInventory;
        public EffectsController EffectsController => DefaultPlayerMotor.EffectsController;


        public IsGroundedController IsGroundedController => DefaultPlayerMotor.IsGroundedController;
    }
}
