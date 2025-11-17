using Game.Gameplay.Controls;
using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.HealthHandling;
using Game.Gameplay.GameplayInteractionsSystems.InteractionSystem;
using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.PlayerCharacter.FallingDamage;
using Game.Gameplay.PlayerCharacter.Movement;
using Game.Gameplay.PlayerCharacter.SkillsIntegration;
using Game.Gameplay.VehiclesSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter
{
    public class PlayerCharacter : NetworkBehaviour, 
        IPlayerCharacterMovementControllerHolder, 
        IRigidbodyHolder,
        ISkillCasterHolder,
        ISkillsInventoryHolder, 
        IEffectsControllerHolder,
        IPlayerCharacterAnimationControllerHolder,
        IIsGroundedControllerHolder,
        IHealthControllerHolder,
        IInteractorHolder,
        IFallingSpeedControllerHolder,
        IVehicleControllerHolder
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
        public EffectsController EffectsController { get; private set; }
        [field: SerializeField]
        public PlayerCharacterAnimationController PlayerCharacterAnimationController { get; private set; }
        [field: SerializeField]
        public IsGroundedController IsGroundedController { get; private set; }
        [field: SerializeField]
        public HealthController HealthController { get; private set; }
        [field: SerializeField]
        public Interactor Interactor { get; private set; }
        [field: SerializeField]
        public FallingSpeedController FallingSpeedController { get; private set; }
        [field: SerializeField]
        public VehicleController VehicleController { get; private set; }

        [field: Header("Gameplay Rules Handlers")]
        [field: SerializeField]
        public FallingDamageHandler FallingDamageHandler { get; private set; }
        
        private void Awake()
        {
            // Initializing Controllers
            IsGroundedController.SetRelativeSource(Rigidbody.transform);
            FallingSpeedController.SetDependencies(IsGroundedController, MovementController.Blackboard);
            MovementController.SetDependencies(Rigidbody, FallingSpeedController, IsGroundedController);
            Interactor.SetSource(this);
            VehicleController.SetDependencies(this);
            
            // Initializing Rules Handlers
            FallingDamageHandler.SetDependencies(IsGroundedController, FallingSpeedController, HealthController);
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            if (IsOwner)
                LocalPlayerController.Instance.AssignCharacter(this);
        }
    }
}
