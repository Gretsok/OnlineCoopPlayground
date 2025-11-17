using System;
using System.Collections.Generic;
using Game.Gameplay.PlayerCharacter.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    [RequireComponent(typeof(Vehicle))]
    public class HangGliderCharactersSeatsController : NetworkBehaviour
    {
        [SerializeField]
        private HangGliderCharacterMovementBehaviour m_movementBehaviourPrefab;
        
        private readonly Dictionary<VehicleController, HangGliderCharacterMovementBehaviour> m_movementBehaviours_ServerOnly = new ();
        
        private Vehicle m_vehicle;

        public void SetDependencies(Vehicle a_vehicle)
        {
            m_vehicle = a_vehicle;
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            
            m_vehicle.OnCharacterJoined_ServerCalled += HandleCharacterJoined_ServerCalled;
            m_vehicle.OnCharacterLeft_ServerCalled += HandleCharacterLeft_ServerCalled;
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            
            m_vehicle.OnCharacterJoined_ServerCalled -= HandleCharacterJoined_ServerCalled;
            m_vehicle.OnCharacterLeft_ServerCalled -= HandleCharacterLeft_ServerCalled;
        }

        private void HandleCharacterJoined_ServerCalled(VehicleController a_obj)
        {
            var movementController = (a_obj.Parent as IPlayerCharacterMovementControllerHolder).MovementController;
            
            var movementBehaviour = Instantiate(m_movementBehaviourPrefab);
            movementBehaviour.NetworkObject.SpawnWithOwnership(a_obj.OwnerClientId);
            movementBehaviour.NetworkObject.TrySetParent(movementController.gameObject);

            movementController.StackBehaviour_ForServer(movementBehaviour);
        }
        
        private void HandleCharacterLeft_ServerCalled(VehicleController a_obj)
        {
            var movementController = (a_obj.Parent as IPlayerCharacterMovementControllerHolder).MovementController;

            var movementBehaviour = m_movementBehaviours_ServerOnly[a_obj];
            m_movementBehaviours_ServerOnly.Remove(a_obj);
            
            movementController.UnstackBehaviour_ForServer(movementBehaviour);
            
            // We destroy the behaviour in 5 seconds to ensure that the clients will correctly leave it before deleting it.
            Destroy(movementController.gameObject, 5f);
        }
    }
}
