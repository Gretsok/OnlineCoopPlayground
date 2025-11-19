using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay.VehiclesSystem
{
    /// <summary>
    /// Can transport one or several players.
    /// </summary>
    public class Vehicle : NetworkBehaviour
    {
        [field: SerializeField]
        public VehicleSeatsController VehicleSeatsController { get; private set; }
        [field: SerializeField]
        public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField]
        public VehicleLocalCameraController LocalCameraController { get; private set; }
        
        public delegate bool DCondition(VehiclePassengerController a_passengerController);

        private readonly List<DCondition> m_conditions = new List<DCondition>();
        
        public void AddCondition(DCondition a_condition)
        {
            if (!m_conditions.Contains(a_condition))
                m_conditions.Add(a_condition);
        }

        public void RemoveCondition(DCondition a_condition)
        {
            m_conditions.RemoveAll(a_existingCondition => a_existingCondition == a_condition);
        }
        
        /// <summary>
        /// Should only be called from <see cref="VehiclePassengerController"/>
        /// </summary>
        /// <param name="a_vehiclePassengerController"></param>
        /// <returns></returns>
        public bool CanHopIn_ForServer(VehiclePassengerController a_vehiclePassengerController)
        {
            if (!a_vehiclePassengerController)
                return false;
            
            for (int i = 0; i < m_conditions.Count; i++)
            {
                var condition = m_conditions[i];

                if (!condition(a_vehiclePassengerController))
                    return false;
            }
            return true;
        }
        
        private readonly NetworkList<NetworkBehaviourReference> m_charactersInVehicle = new NetworkList<NetworkBehaviourReference>();

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            AddCondition(IsMaxCharactersInVehicleNotReached);
            AddCondition(CharacterNotInThisVehicle);
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            RemoveCondition(IsMaxCharactersInVehicleNotReached);
            RemoveCondition(CharacterNotInThisVehicle);

            if (IsServer)
            {
                for (var i = 0; i < m_charactersInVehicle.Count; i++)
                {
                    var vehiclePassengerControllerReference = m_charactersInVehicle[i];
                    if (vehiclePassengerControllerReference.TryGet(out VehiclePassengerController vehiclePassengerController))
                        KickCharacterFromVehicle_ForServer(vehiclePassengerController);
                }
            }
        }

        private bool IsMaxCharactersInVehicleNotReached(VehiclePassengerController a_passengerController)
        {
            return m_charactersInVehicle.Count < VehicleSeatsController.MaximumSeats;
        }

        private bool CharacterNotInThisVehicle(VehiclePassengerController a_passengerController)
        {
            return !m_charactersInVehicle.Contains(a_passengerController);
        }

        /// <summary>
        /// Should only be called from <see cref="VehiclePassengerController"/>
        /// </summary>
        /// <param name="a_vehiclePassengerController"></param>
        public bool RequestCharacterToJoin_ForServer(VehiclePassengerController a_vehiclePassengerController)
        {
            if (!IsServer)
                return false;
            
            if (!CanHopIn_ForServer(a_vehiclePassengerController))
                return false;
            
            
            m_charactersInVehicle.Add(a_vehiclePassengerController);
            HandleCharacterJoined_ServerCalled(a_vehiclePassengerController);
            OnCharacterJoined_ServerCalled?.Invoke(a_vehiclePassengerController);
            m_onCharacterJoined_ServerCalled?.Invoke();
            HandleCharacterJoined_ClientsRpc(a_vehiclePassengerController.NetworkObject, 
                a_vehiclePassengerController.NetworkObject.GetNetworkBehaviourOrderIndex(a_vehiclePassengerController));
            
            Debug.Log($"[SERVER] {a_vehiclePassengerController.Parent.name} joined vehicle {gameObject.name}.", a_vehiclePassengerController.Parent.gameObject);
            return true;
        }

        [SerializeField]
        private UnityEvent m_onCharacterJoined_ServerCalled;
        public event Action<VehiclePassengerController> OnCharacterJoined_ServerCalled;
        protected virtual void HandleCharacterJoined_ServerCalled(VehiclePassengerController a_vehiclePassengerController)
        { }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleCharacterJoined_ClientsRpc(NetworkObjectReference a_networkObjectReference,
            ushort a_componentOrderIndex)
        {
            if (!a_networkObjectReference.TryGet(out NetworkObject networkObject))
                return;
            var vehicleController = networkObject.GetNetworkBehaviourAtOrderIndex(a_componentOrderIndex) as VehiclePassengerController;
            
            HandleCharacterJoined_ClientsCalled(vehicleController);
            OnCharacterJoined_ClientsCalled?.Invoke(vehicleController);
            m_onCharacterJoined_ClientsCalled?.Invoke();
        }
        
        [SerializeField]
        private UnityEvent m_onCharacterJoined_ClientsCalled;
        public event Action<VehiclePassengerController> OnCharacterJoined_ClientsCalled;
        protected virtual void HandleCharacterJoined_ClientsCalled(VehiclePassengerController a_vehiclePassengerController)
        { }

        public event Action<Vehicle, VehiclePassengerController> OnCharacterKicked_ServerCalled;
        public void KickCharacterFromVehicle_ForServer(VehiclePassengerController a_vehiclePassengerController)
        {
            if (!IsServer)
                return;
            
            if (!m_charactersInVehicle.Contains(a_vehiclePassengerController))
            {
                Debug.LogError($"[SERVER] Cannot kick this character from this vehicle: The character is not found in the vehicle.", 
                    a_vehiclePassengerController.gameObject);
                return;
            }
            
            m_charactersInVehicle.Remove(a_vehiclePassengerController);
            HandleCharacterLeft_ForServer(a_vehiclePassengerController);
            
            OnCharacterKicked_ServerCalled?.Invoke(this, a_vehiclePassengerController);
        }
        
        private void HandleCharacterLeft_ForServer(VehiclePassengerController a_vehiclePassengerController)
        {
            HandleCharacterLeft_ServerCalled(a_vehiclePassengerController);
            OnCharacterLeft_ServerCalled?.Invoke(a_vehiclePassengerController);
            m_onCharacterLeft_ServerCalled?.Invoke();
            HandleCharacterLeft_ClientsRpc(a_vehiclePassengerController.NetworkObject, 
                a_vehiclePassengerController.NetworkObject.GetNetworkBehaviourOrderIndex(a_vehiclePassengerController));
            
            Debug.Log($"[SERVER] {a_vehiclePassengerController.Parent.name} left vehicle {gameObject.name}.", a_vehiclePassengerController.Parent.gameObject);
        }
        
        [SerializeField]
        private UnityEvent m_onCharacterLeft_ServerCalled;
        public event Action<VehiclePassengerController> OnCharacterLeft_ServerCalled;
        protected virtual void HandleCharacterLeft_ServerCalled(VehiclePassengerController a_vehiclePassengerController)
        { }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleCharacterLeft_ClientsRpc(NetworkObjectReference a_networkObjectReference,
            ushort a_componentOrderIndex)
        {
            if (!a_networkObjectReference.TryGet(out NetworkObject networkObject))
                return;
            var vehicleController = networkObject.GetNetworkBehaviourAtOrderIndex(a_componentOrderIndex) as VehiclePassengerController;
            
            HandleCharacterLeft_ClientsCalled(vehicleController);
            OnCharacterLeft_ClientsCalled?.Invoke(vehicleController);
            m_onCharacterLeft_ClientsCalled?.Invoke();
        }
        
        [SerializeField]
        private UnityEvent m_onCharacterLeft_ClientsCalled;
        public event Action<VehiclePassengerController> OnCharacterLeft_ClientsCalled;
        protected virtual void HandleCharacterLeft_ClientsCalled(VehiclePassengerController a_vehiclePassengerController)
        { }
        
    }
}
