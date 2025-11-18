using System;
using Game.Gameplay.CameraSystem;
using Game.Gameplay.CharactersManagement;
using Game.Gameplay.LocalControls;
using Tools.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.PlayerCharacter.MotorImplementations.Default
{
    public class DefaultLocalPlayerInputProcessor : ALocalPlayerInputProcessor
    {
        
        public Action<DefaultLocalPlayerInputProcessor> OnInputActionsInitialized;
        
        private CameraController m_cameraController;
        
        public DefaultPlayerMotor AssignedMotor { get; private set; }

        public event Action<DefaultLocalPlayerInputProcessor> OnCharacterAssigned;
        public void AssignCharacter(DefaultPlayerMotor a_motor)
        {
            if (!a_motor.IsOwner)
            {
                Debug.LogError($"Trying to assign a character that is not owned locally.");
                return;
            }
        
            AssignedMotor = a_motor;
            OnCharacterAssigned?.Invoke(this);
            
            if (!m_cameraController)
                m_cameraController = CameraController.Instance;
            
            m_cameraController.AssignCameraTarget(a_motor.transform);
        }

        protected override void HandleActivation()
        {
            base.HandleActivation();
            if (!m_cameraController)
                m_cameraController = CameraController.Instance;            
            
            PlayersCharactersManager.Instance.RequestMotorFor_ForClients(NetworkManager.Singleton.LocalClientId, a_motor =>
            {
                if (a_motor is DefaultPlayerMotor defaultPlayerMotor)
                    AssignCharacter(defaultPlayerMotor);
            });
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Actions.Movement.Jump.started += HandleJumpInputStarted;
            Actions.Movement.Jump.canceled += HandleJumpInputCanceled;
            Actions.Movement.Crouch.started += HandleCrouchInputStarted;
            Actions.Movement.Crouch.canceled += HandleCrouchInputCanceled;

            Actions.Interaction.Interact.started += HandleInteractInputStarted;
            Actions.Interaction.Skill_1.started += HandleSkill1InputStarted;
            Actions.Interaction.Skill_2.started += HandleSkill2InputStarted;
            Actions.Interaction.Skill_3.started += HandleSkill3InputStarted;
        
            OnInputActionsInitialized?.Invoke(this);
        }
        private void HandleJumpInputStarted(InputAction.CallbackContext a_obj)
        {
            if (!AssignedMotor)
                return;
            
            AssignedMotor.MovementController?.Blackboard.StartJump();
        }

        private void HandleJumpInputCanceled(InputAction.CallbackContext a_obj)
        {
            if (!AssignedMotor)
                return;
            
            AssignedMotor.MovementController?.Blackboard.StopJump();
        }

        private void HandleCrouchInputStarted(InputAction.CallbackContext a_obj)
        {
        }

        private void HandleCrouchInputCanceled(InputAction.CallbackContext a_obj)
        {
        }
        
        private void HandleInteractInputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedMotor.Interactor.TryToInteract_ForOwner();
        }

        private void HandleSkill1InputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedMotor.SkillCaster.TryToTriggerSkill_ForOwner(AssignedMotor.SkillsInventory.GetSkillByIndex(0));
        }

        private void HandleSkill2InputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedMotor.SkillCaster.TryToTriggerSkill_ForOwner(AssignedMotor.SkillsInventory.GetSkillByIndex(1));
        }

        private void HandleSkill3InputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedMotor.SkillCaster.TryToTriggerSkill_ForOwner(AssignedMotor.SkillsInventory.GetSkillByIndex(2));
        }

        protected override void UpdateInput()
        {
            base.UpdateInput();
            if (!AssignedMotor)
                return;

            var moveInput = Actions.Movement.Move.ReadValue<Vector2>();
            var lookAroundInput = Actions.Camera.LookAround.ReadValue<Vector2>();

            var forward = m_cameraController.CameraAnchor.transform.forward.Flatten().normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            AssignedMotor.MovementController.Blackboard.SetDirectionInput(moveInput.x * right + moveInput.y * forward);
            
            m_cameraController.SetLookAroundInput(lookAroundInput);
        }
    }
}