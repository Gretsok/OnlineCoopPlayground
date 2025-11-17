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
                a_vehicle.OnCharacterKicked_ServerCalled += HandleCharacterKickedServerCalled;
            }
        }

        private void HandleCharacterKickedServerCalled(Vehicle a_vehicle, VehiclePassengerController a_vehiclePassengerController)
        {
            if (!IsServer)
            {
                return;
            }
            
            if (!m_currentVehicle.Value.TryGet(out Vehicle currentVehicle))
                return;

            if (a_vehicle != currentVehicle)
            {
                a_vehicle.OnCharacterKicked_ServerCalled -= HandleCharacterKickedServerCalled;
                return;
            }

            if (a_vehiclePassengerController != this)
                return;
            
            m_currentVehicle.Value = null;
            a_vehicle.OnCharacterKicked_ServerCalled -= HandleCharacterKickedServerCalled;
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
