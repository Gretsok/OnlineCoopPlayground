using Game.Gameplay.GameplayInteractionsSystems.InteractionSystem;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    public class HangGlider : MonoBehaviour
    {
        [SerializeField]
        private Vehicle m_vehicle;
        [SerializeField]
        private Interactable m_interactable;

        [SerializeField]
        private HangGliderCharactersSeatsController m_seatsController;
        [SerializeField]
        private HangGliderVehicleMovementController m_movementController;

        private void Awake()
        {
            m_seatsController.SetDependencies(m_vehicle);
            m_movementController.SetDependencies(m_seatsController);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!m_vehicle)
                m_vehicle = GetComponent<Vehicle>();
            if (!m_interactable)
                m_interactable = GetComponent<Interactable>();
        }
#endif
    }
}
