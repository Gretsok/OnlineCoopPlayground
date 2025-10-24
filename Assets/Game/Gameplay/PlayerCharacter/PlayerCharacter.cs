using Game.Playground.PlayerCharacter.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Game.Playground.PlayerCharacter
{
    public class PlayerCharacter : NetworkBehaviour, 
        IPlayerCharacterMovementControllerHolder, 
        IRigidbodyHolder
    {
         [field: SerializeField]
         public Rigidbody Rigidbody { get; private set; }
         [field: SerializeField]
         public PlayerCharacterMovementController MovementController { get; private set; }

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
