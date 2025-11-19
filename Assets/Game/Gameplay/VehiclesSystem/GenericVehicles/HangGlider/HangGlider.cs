using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    public class HangGlider : Vehicle
    {
        [field: SerializeField]
        //public FullPhysicsTest1_HangGliderVehicleMovementController MovementController { get; private set; }
        public HangGliderVehicleMovementController MovementController { get; private set; }
        [field: SerializeField]
        public NetworkObject Model { get; private set; }
        private void Awake()
        {
            VehicleSeatsController.SetDependencies(Model);
            MovementController.SetDependencies(VehicleSeatsController, Rigidbody, Model);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!Model.IsSpawned)
                Model.SpawnWithOwnership(OwnerClientId);
            
            VehicleSeatsController.SetDependencies(Model);
            MovementController.SetDependencies(VehicleSeatsController, Rigidbody, Model);
        }
    }
}
