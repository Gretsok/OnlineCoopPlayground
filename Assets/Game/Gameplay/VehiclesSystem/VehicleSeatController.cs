using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem
{
    /// <summary>
    /// Spawn & Handle players vehicles motors.
    /// Should position the player motors correctly.
    /// </summary>
    public class VehicleSeatController : NetworkBehaviour
    {
        [SerializeField]
        private List<Transform> m_vehicleSeatsAnchors = new();

        [SerializeField]
        private NetworkObject m_vehicleSeatsNetworkedAnchorPrefab;
    }
}
