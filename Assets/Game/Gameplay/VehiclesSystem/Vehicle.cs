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
        public delegate bool DCondition(VehicleController a_controller);

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
        /// Should only be called from <see cref="VehicleController"/>
        /// </summary>
        /// <param name="a_vehicleController"></param>
        /// <returns></returns>
        public bool CanHopIn_ForServer(VehicleController a_vehicleController)
        {
            if (!a_vehicleController)
                return false;
            
            for (int i = 0; i < m_conditions.Count; i++)
            {
                var condition = m_conditions[i];

                if (!condition(a_vehicleController))
                    return false;
            }
            return true;
        }
        
        [SerializeField]
        private int m_maxCharactersInVehicle = 1;
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
        }

        private bool IsMaxCharactersInVehicleNotReached(VehicleController a_controller)
        {
            return m_charactersInVehicle.Count < m_maxCharactersInVehicle;
        }

        private bool CharacterNotInThisVehicle(VehicleController a_controller)
        {
            return !m_charactersInVehicle.Contains(a_controller);
        }

        /// <summary>
        /// Should only be called from <see cref="VehicleController"/>
        /// </summary>
        /// <param name="a_vehicleController"></param>
        public bool RequestCharacterToJoin_ForServer(VehicleController a_vehicleController)
        {
            if (!CanHopIn_ForServer(a_vehicleController))
                return false;
            
            
            m_charactersInVehicle.Add(a_vehicleController);
            HandleCharacterJoined_ServerCalled(a_vehicleController);
            OnCharacterJoined_ServerCalled?.Invoke(a_vehicleController);
            m_onCharacterJoined_ServerCalled?.Invoke();
            HandleCharacterJoined_ClientsRpc(a_vehicleController.NetworkObject, 
                a_vehicleController.NetworkObject.GetNetworkBehaviourOrderIndex(a_vehicleController));
            
            Debug.Log($"[SERVER] {a_vehicleController.Parent.name} joined vehicle {gameObject.name}.", a_vehicleController.Parent.gameObject);
            return true;
        }

        [SerializeField]
        private UnityEvent m_onCharacterJoined_ServerCalled;
        public event Action<VehicleController> OnCharacterJoined_ServerCalled;
        protected virtual void HandleCharacterJoined_ServerCalled(VehicleController a_vehicleController)
        { }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleCharacterJoined_ClientsRpc(NetworkObjectReference a_networkObjectReference,
            ushort a_componentOrderIndex)
        {
            if (!a_networkObjectReference.TryGet(out NetworkObject networkObject))
                return;
            var vehicleController = networkObject.GetNetworkBehaviourAtOrderIndex(a_componentOrderIndex) as VehicleController;
            
            HandleCharacterJoined_ClientsCalled(vehicleController);
            OnCharacterJoined_ClientsCalled?.Invoke(vehicleController);
            m_onCharacterJoined_ClientsCalled?.Invoke();
        }
        
        [SerializeField]
        private UnityEvent m_onCharacterJoined_ClientsCalled;
        public event Action<VehicleController> OnCharacterJoined_ClientsCalled;
        protected virtual void HandleCharacterJoined_ClientsCalled(VehicleController a_vehicleController)
        { }

        public event Action<Vehicle, VehicleController> OnCharacterKicked_ServerCalled;
        public void KickCharacterFromVehicle_ForServer(VehicleController a_vehicleController)
        {
            if (!IsServer)
                return;
            
            if (!m_charactersInVehicle.Contains(a_vehicleController))
            {
                Debug.LogError($"[SERVER] Cannot kick this character from this vehicle: The character is not found in the vehicle.", 
                    a_vehicleController.gameObject);
                return;
            }
            
            m_charactersInVehicle.Remove(a_vehicleController);
            HandleCharacterLeft_ForServer(a_vehicleController);
            
            OnCharacterKicked_ServerCalled?.Invoke(this, a_vehicleController);
        }
        
        private void HandleCharacterLeft_ForServer(VehicleController a_vehicleController)
        {
            HandleCharacterLeft_ServerCalled(a_vehicleController);
            OnCharacterLeft_ServerCalled?.Invoke(a_vehicleController);
            m_onCharacterLeft_ServerCalled?.Invoke();
            HandleCharacterLeft_ClientsRpc(a_vehicleController.NetworkObject, 
                a_vehicleController.NetworkObject.GetNetworkBehaviourOrderIndex(a_vehicleController));
            
            Debug.Log($"[SERVER] {a_vehicleController.Parent.name} left vehicle {gameObject.name}.", a_vehicleController.Parent.gameObject);
        }
        
        [SerializeField]
        private UnityEvent m_onCharacterLeft_ServerCalled;
        public event Action<VehicleController> OnCharacterLeft_ServerCalled;
        protected virtual void HandleCharacterLeft_ServerCalled(VehicleController a_vehicleController)
        { }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleCharacterLeft_ClientsRpc(NetworkObjectReference a_networkObjectReference,
            ushort a_componentOrderIndex)
        {
            if (!a_networkObjectReference.TryGet(out NetworkObject networkObject))
                return;
            var vehicleController = networkObject.GetNetworkBehaviourAtOrderIndex(a_componentOrderIndex) as VehicleController;
            
            HandleCharacterLeft_ClientsCalled(vehicleController);
            OnCharacterLeft_ClientsCalled?.Invoke(vehicleController);
            m_onCharacterLeft_ClientsCalled?.Invoke();
        }
        
        [SerializeField]
        private UnityEvent m_onCharacterLeft_ClientsCalled;
        public event Action<VehicleController> OnCharacterLeft_ClientsCalled;
        protected virtual void HandleCharacterLeft_ClientsCalled(VehicleController a_vehicleController)
        { }
    }
}
