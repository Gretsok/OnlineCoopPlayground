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

        private void HandleCharacterJoined_ServerCalled(VehiclePassengerController a_obj)
        {

        }
        
        private void HandleCharacterLeft_ServerCalled(VehiclePassengerController a_obj)
        {

        }
    }
}
