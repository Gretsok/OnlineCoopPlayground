using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    public class HangGliderVehicleMovementController : MonoBehaviour
    {
        private HangGliderCharactersSeatsController m_seatsController;

        public void SetDependencies(HangGliderCharactersSeatsController a_seatsController)
        {
            m_seatsController = a_seatsController;
        }
    }
}
