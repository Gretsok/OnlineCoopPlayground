using System;
using Game.Gameplay.CameraSystem;
using Game.Playground.Controls;
using Tools.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Controls
{
    public class LocalPlayerController : MonoBehaviour
    {
        public static LocalPlayerController Instance { get; private set; }
    
        public PlayerActions Actions { get; private set; }

        public Action<LocalPlayerController> OnInputActionsInitialized;
        
        private CameraController m_cameraController;
    
        private void Awake()
        {
            Instance = this;
        }

    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (!m_cameraController)
                m_cameraController = CameraController.Instance;            
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Actions = new PlayerActions();
            Actions.Enable();

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




        private void OnDestroy()
        {
            Actions.Disable();
            Actions.Dispose();
        }

        public PlayerCharacter.PlayerCharacter AssignedCharacter { get; private set; }

        public event Action<LocalPlayerController> OnCharacterAssigned;
        public void AssignCharacter(PlayerCharacter.PlayerCharacter a_character)
        {
            if (!a_character.IsOwner)
            {
                Debug.LogError($"Trying to assign a character that is not owned locally.");
                return;
            }
        
            AssignedCharacter = a_character;
            OnCharacterAssigned?.Invoke(this);
            
            if (!m_cameraController)
                m_cameraController = CameraController.Instance;
            
            m_cameraController.AssignCameraTarget(a_character.transform);
        }
    
        private void HandleJumpInputStarted(InputAction.CallbackContext a_obj)
        {
            if (!AssignedCharacter)
                return;
            
            AssignedCharacter.MovementController?.Jump();
        }

        private void HandleJumpInputCanceled(InputAction.CallbackContext a_obj)
        {
        }

        private void HandleCrouchInputStarted(InputAction.CallbackContext a_obj)
        {
        }

        private void HandleCrouchInputCanceled(InputAction.CallbackContext a_obj)
        {
        }
        
        private void HandleInteractInputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedCharacter.Interactor.TryToInteract_ForOwner();
        }

        private void HandleSkill1InputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedCharacter.SkillCaster.TryToTriggerSkill_ForOwner(AssignedCharacter.SkillsInventory.GetSkillByIndex(0));
        }

        private void HandleSkill2InputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedCharacter.SkillCaster.TryToTriggerSkill_ForOwner(AssignedCharacter.SkillsInventory.GetSkillByIndex(1));
        }

        private void HandleSkill3InputStarted(InputAction.CallbackContext a_obj)
        {
            AssignedCharacter.SkillCaster.TryToTriggerSkill_ForOwner(AssignedCharacter.SkillsInventory.GetSkillByIndex(2));
        }


        
        private void Update()
        {
            if (Actions == null) 
                return;
            /*if (!Application.isFocused)
            return;*/
            if (!AssignedCharacter)
                return;

            var moveInput = Actions.Movement.Move.ReadValue<Vector2>();
            var lookAroundInput = Actions.Camera.LookAround.ReadValue<Vector2>();

            var forward = m_cameraController.CameraAnchor.transform.forward.Flatten().normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            AssignedCharacter.MovementController.SetDirectionInput(moveInput.x * right + moveInput.y * forward);
            
            m_cameraController.SetLookAroundInput(lookAroundInput);
        }
    }
}
