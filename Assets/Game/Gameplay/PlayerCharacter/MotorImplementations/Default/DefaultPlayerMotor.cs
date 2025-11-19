using DG.Tweening;
using Game.Gameplay.GameplayInteractionsSystems.InteractionSystem;
using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Game.Gameplay.PlayerCharacter.FallingDamage;
using Game.Gameplay.PlayerCharacter.Movement;
using Game.Gameplay.PlayerCharacter.Movement.IsGroundedControl;
using Game.Gameplay.PlayerCharacter.SkillsIntegration;
using Tools.Utils;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.MotorImplementations.Default
{
    public class DefaultPlayerMotor : APlayerMotor, 
        IPlayerCharacterMovementControllerHolder, 
        IRigidbodyHolder,
        ISkillCasterHolder,
        ISkillsInventoryHolder, 
        IIsGroundedControllerHolder,
        IInteractorHolder,
        IFallingSpeedControllerHolder
    {
        [field: Header("Core Character Controllers")]
        [field: SerializeField]
        public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField]
        public PlayerCharacterMovementController MovementController { get; private set; }
        [field: SerializeField]
        public SkillCaster SkillCaster { get; private set; }
        [field: SerializeField]
        public SkillsInventory SkillsInventory { get; private set; }
        [field: SerializeField]
        public IsGroundedController IsGroundedController { get; private set; }
        [field: SerializeField]
        public Interactor Interactor { get; private set; }
        [field: SerializeField]
        public FallingSpeedController FallingSpeedController { get; private set; }

        [field: Header("Gameplay Rules Handlers")]
        [field: SerializeField]
        public FallingDamageHandler FallingDamageHandler { get; private set; }


        protected override void HandleSetUpCustomLogic_ServerCalled()
        {
            base.HandleSetUpCustomLogic_ServerCalled();

            // Getting up if the character does not stand up.
            if (Vector3.Angle(Vector3.up, transform.up) > 20f)
            {
                var planarForward = transform.forward.Flatten();
                if (planarForward.sqrMagnitude == 0)
                    planarForward = transform.up.Flatten();

                transform.DORotateQuaternion(Quaternion.LookRotation(planarForward, Vector3.up), 0.5f).SetEase(Ease.InOutQuint);
            }
        }

        protected override void SetUpDependencies()
        {
            base.SetUpDependencies();
            
            // Initializing Controllers
            IsGroundedController.SetRelativeSource(Rigidbody.transform);
            FallingSpeedController.SetDependencies(IsGroundedController, MovementController.Blackboard);
            MovementController.SetDependencies(Rigidbody, FallingSpeedController, IsGroundedController);
            Interactor.SetSource(this);
            
            // Initializing Rules Handlers
            FallingDamageHandler.SetDependencies(IsGroundedController, FallingSpeedController, HealthController);

            Debug.Log($"Default player motor's dependencies initialized.");
        }
    }
}
