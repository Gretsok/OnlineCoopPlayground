using Game.Gameplay.GameplayInteractionsSystems.InteractionSystem;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.Interaction
{
    public class JoinVehicleInteractableComponent : MonoBehaviour
    {
        [SerializeField]
        private Interactable m_interactable;
        [SerializeField]
        private Vehicle m_vehicle;

        private void Awake()
        {
            if (!m_interactable)
                m_interactable = GetComponent<Interactable>();
            if (!m_vehicle)
                m_vehicle = GetComponent<Vehicle>();
        }

        private void Start()
        {
            m_interactable.AddCondition(CanInteractorHopInVehicle);
            m_interactable.OnInteractionRequested_ServerCalled += HandleInteractionRequested_ServerCalled;
        }

        private void OnDestroy()
        {
            m_interactable.OnInteractionRequested_ServerCalled -= HandleInteractionRequested_ServerCalled;
            m_interactable.RemoveCondition(CanInteractorHopInVehicle);
        }

        private bool CanInteractorHopInVehicle(Interactor a_interactor)
        {
            return m_vehicle.CanHopIn_ForServer((a_interactor.Source as IVehicleControllerHolder)?.VehicleController);
        }

        private void HandleInteractionRequested_ServerCalled(Interactable a_arg1, Interactor a_arg2)
        {
            (a_arg2.Source as IVehicleControllerHolder).VehicleController.JoinVehicle_ForServer(m_vehicle);
        }
    }
}
