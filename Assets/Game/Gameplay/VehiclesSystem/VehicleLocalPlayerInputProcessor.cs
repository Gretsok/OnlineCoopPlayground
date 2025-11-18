using Game.Gameplay.CameraSystem;
using Game.Gameplay.LocalControls;
using Tools.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.VehiclesSystem
{
    public class VehicleLocalPlayerInputProcessor : ALocalPlayerInputProcessor
    {
        private CameraController m_cameraController;
        public Vehicle Vehicle { get; private set; }
        public VehiclePassengerController PassengerController { get; private set; }
        public void SetVehicle(Vehicle a_vehicle, VehiclePassengerController a_passengerController)
        {
            if (!m_cameraController)
                m_cameraController = CameraController.Instance;    
            
            Vehicle = a_vehicle;
            PassengerController = a_passengerController;
            
            if (Vehicle)
                m_cameraController.AssignCameraTarget(Vehicle.transform);
        }
        
        protected override void HandleActivation()
        {
            base.HandleActivation();
            
            if (!m_cameraController)
                m_cameraController = CameraController.Instance;    
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Actions.Interaction.Interact.started += HandleInteractInputStarted;
        }

        private void HandleInteractInputStarted(InputAction.CallbackContext a_obj)
        {
            PassengerController.LeaveVehicle_ForOwner();
        }
        
        protected override void UpdateInput()
        {
            base.UpdateInput();
            if (!Vehicle)
                return;

            var moveInput = Actions.Movement.Move.ReadValue<Vector2>();
            var lookAroundInput = Actions.Camera.LookAround.ReadValue<Vector2>();

            var forward = m_cameraController.CameraAnchor.transform.forward.Flatten().normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            //AssignedMotor.MovementController.Blackboard.SetDirectionInput(moveInput.x * right + moveInput.y * forward);
            
            m_cameraController.SetLookAroundInput(lookAroundInput);
        }
    }
}
