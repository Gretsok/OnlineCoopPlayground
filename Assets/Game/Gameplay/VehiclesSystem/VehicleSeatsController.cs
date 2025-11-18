using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Game.Gameplay.CharactersManagement;
using Game.Gameplay.VehiclesSystem.PlayerMotor;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Gameplay.VehiclesSystem
{
    /// <summary>
    /// Should position the player motors correctly.
    /// </summary>
    public class VehicleSeatsController : NetworkBehaviour
    {
        [Serializable]
        public class SeatInfo
        {
            public NetworkObject NetworkObjectAnchor;
            public VehiclePlayerMotor Passenger;
        }
        
        [SerializeField]
        private Vehicle m_vehicle;
        
        [SerializeField]
        private List<Transform> m_vehicleSeatsAnchorsPositionners = new();

        [SerializeField]
        private NetworkObject m_vehicleSeatsNetworkedAnchorPrefab;

        private readonly List<SeatInfo> m_seatInfos_ServerOnly = new();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsServer)
                return;

            for (int i = 0; i < m_vehicleSeatsAnchorsPositionners.Count; i++)
            {
                var anchorPositionner = m_vehicleSeatsAnchorsPositionners[i];
                var anchor = NetworkManager.SpawnManager
                    .InstantiateAndSpawn(m_vehicleSeatsNetworkedAnchorPrefab, 
                        position: anchorPositionner.position, 
                        rotation: anchorPositionner.rotation);
                anchor.TrySetParent(NetworkObject);
                m_seatInfos_ServerOnly.Add(new SeatInfo()
                {
                    NetworkObjectAnchor = anchor,
                });
            }
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            for (int i = m_seatInfos_ServerOnly.Count - 1; i >= 0; i--)
            {
                var anchor = m_seatInfos_ServerOnly[i].NetworkObjectAnchor;
                Destroy(anchor.gameObject);
            }
        }

        private void Start()
        {
            m_vehicle.OnCharacterJoined_ServerCalled += HandleCharacterJoined_ServerCalled;
            m_vehicle.OnCharacterLeft_ServerCalled += HandleCharacterLeft_ServerCalled;
        }

        private void HandleCharacterJoined_ServerCalled(VehiclePassengerController a_obj)
        {
            StartCoroutine(AddingPlayerToVehicleAsync_ServerOnly(a_obj));
        }

        private IEnumerator AddingPlayerToVehicleAsync_ServerOnly(VehiclePassengerController a_obj)
        {
            yield return new WaitUntil(() => a_obj.Parent as VehiclePlayerMotor);
            var vehiclePlayerMotor = (a_obj.Parent as VehiclePlayerMotor);
            yield return new WaitUntil(() => vehiclePlayerMotor.HasBeenSetUp_ServerOnly);

            var seat = m_seatInfos_ServerOnly.First(a_seat => !a_seat.Passenger);

            vehiclePlayerMotor.NetworkObject.TrySetParent(seat.NetworkObjectAnchor);
            seat.Passenger = vehiclePlayerMotor;
            vehiclePlayerMotor.transform.DOLocalMove(default, 0.3f).SetEase(Ease.InOutBack);
            vehiclePlayerMotor.transform.DOLocalRotate(default, 0.3f).SetEase(Ease.InOutBack);
        }
        
        private void HandleCharacterLeft_ServerCalled(VehiclePassengerController a_obj)
        {
            
        }
    }
}
