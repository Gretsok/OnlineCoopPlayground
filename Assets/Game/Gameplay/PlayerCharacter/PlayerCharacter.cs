using Game.Gameplay.Controls;
using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.PlayerCharacter.Movement;
using Game.Gameplay.PlayerCharacter.SkillsIntegration;
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
        IPlayerCharacterAnimationControllerHolder
    {
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

         private void Start()
         {
             MovementController.SetDependencies(Rigidbody);
         }

         protected override void OnNetworkPostSpawn()
         {
             base.OnNetworkPostSpawn();
             
             if (IsOwner)
                LocalPlayerController.Instance.AssignCharacter(this);
         }
    }
}
