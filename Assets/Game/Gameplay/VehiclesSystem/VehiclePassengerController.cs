using System;
using Game.Gameplay.CharactersManagement;
using Game.Gameplay.LocalControls;
using Game.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem
{
    /// <summary>
    /// Represents something that go in a vehicle. Mostly a player.
    /// </summary>
    public class VehiclePassengerController : NetworkBehaviour
    {
        private readonly NetworkVariable<NetworkBehaviourReference> m_currentVehicle =
            new NetworkVariable<NetworkBehaviourReference>();
        
        public MonoBehaviour Parent { get; private set; }
        
        public void SetDependencies(MonoBehaviour a_parent)
        {
            Parent = a_parent;
        }

        private void Start()
        {
            m_currentVehicle.OnValueChanged += HandleVehicleChanged;
        }

        private void HandleVehicleChanged(NetworkBehaviourReference a_previousVValue, NetworkBehaviourReference a_newValue)
        {
            if (!IsOwner)
                return;
            var localPlayerController = LocalPlayerController.Instance;
            
            if (a_newValue.TryGet(out Vehicle currentVehicle))
            {
                localPlayerController.VehicleLocalPlayerInputProcessor.SetVehicle(currentVehicle, this);
                localPlayerController.SwitchToProcessor(localPlayerController.VehicleLocalPlayerInputProcessor);
            }
            else
            {
                localPlayerController.SwitchToProcessor(localPlayerController.DefaultLocalPlayerInputProcessor);
                localPlayerController.VehicleLocalPlayerInputProcessor.SetVehicle(null, this);
            }
        }

        public void JoinVehicle_ForServer(Vehicle a_vehicle)
        {
            if (!IsServer)
            {
                return;
            }

            if (m_currentVehicle.Value.TryGet(out Vehicle currentVehicle))
            {
                Debug.Log($"[SERVER] {OwnerClientId} cannot join vehicle {a_vehicle.name} : The player is already in a vehicle.");
                return;
            }
            
            if (a_vehicle.RequestCharacterToJoin_ForServer(this))
            {
                m_currentVehicle.Value = a_vehicle;
                a_vehicle.OnCharacterKicked_ServerCalled += HandleCharacterKicked_ServerCalled;
            }
            else
            {
                Debug.Log($"[SERVER] {OwnerClientId} cannot join vehicle {a_vehicle.name}.");
                return;
            }
            
            PlayersCharactersManager.Instance.ChangeMotorTypeFor_ForServer(
                NetworkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<AbstractConnectedClientObject>(),
                a_newMotor =>
                {
                    Debug.Log($"[SERVER] Player motor successfully changed for {OwnerClientId} when entering vehicle {a_vehicle.name}.", a_newMotor.gameObject);
                },
                PlayersCharactersManager.EPlayerMotorType.Vehicle);
            
            Debug.Log($"[SERVER] {OwnerClientId} has joined vehicle {m_currentVehicle.Value}.");
        }
        
        private void HandleCharacterKicked_ServerCalled(Vehicle a_vehicle, VehiclePassengerController a_vehiclePassengerController)
        {
            if (!IsServer)
            {
                return;
            }
            
            if (!m_currentVehicle.Value.TryGet(out Vehicle currentVehicle))
                return;

            if (a_vehicle != currentVehicle)
            {
                a_vehicle.OnCharacterKicked_ServerCalled -= HandleCharacterKicked_ServerCalled;
                return;
            }

            if (a_vehiclePassengerController != this)
                return;
            
            m_currentVehicle.Value = null;
            a_vehicle.OnCharacterKicked_ServerCalled -= HandleCharacterKicked_ServerCalled;
            
            PlayersCharactersManager.Instance.ChangeMotorTypeFor_ForServer(
                NetworkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<AbstractConnectedClientObject>(),
                a_newMotor =>
                {
                    Debug.Log($"[SERVER] Player motor successfully changed for {OwnerClientId} when leaving vehicle {a_vehicle?.name}.", a_newMotor.gameObject);
                }, 
                PlayersCharactersManager.EPlayerMotorType.Default);
            
            Debug.Log($"[SERVER] {OwnerClientId} has left vehicle {m_currentVehicle.Value}.");
        }

        public void LeaveVehicle_ForOwner()
        {
            if (!IsOwner)
                return;
            
            LeaveVehicle_ServerRpc();
        }

        [Rpc(SendTo.Server, RequireOwnership = true)]
        private void LeaveVehicle_ServerRpc()
        {
            LeaveVehicle_ForServer();
        }
        
        public void LeaveVehicle_ForServer()
        {
            if (!IsServer)
            {
                return;
            }
            
            if (m_currentVehicle.Value.TryGet(out Vehicle vehicle))
            {
                vehicle.KickCharacterFromVehicle_ForServer(this);
            }
        }
    }
}
