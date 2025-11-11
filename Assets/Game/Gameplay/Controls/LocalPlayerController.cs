using System;
using Game.Playground.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Controls
{
    public class LocalPlayerController : MonoBehaviour
    {
        public static LocalPlayerController Instance { get; private set; }
    
        public PlayerActions Actions { get; private set; }

        public Action<LocalPlayerController> OnInputActionsInitialized;
    
        private void Awake()
        {
            Instance = this;
        }

    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Actions = new PlayerActions();
            Actions.Enable();

            Actions.Movement.Jump.started += HandleJumpInputStarted;
            Actions.Movement.Jump.canceled += HandleJumpInputCanceled;
            Actions.Movement.Crouch.started += HandleCrouchInputStarted;
            Actions.Movement.Crouch.canceled += HandleCrouchInputCanceled;

            Actions.Combat.Skill_1.started += HandleSkill1InputStarted;
            Actions.Combat.Skill_2.started += HandleSkill2InputStarted;
            Actions.Combat.Skill_3.started += HandleSkill3InputStarted;
            Actions.Combat.Skill_4.started += HandleSkill4InputStarted;
        
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
        }
    
        private void HandleJumpInputStarted(InputAction.CallbackContext a_obj)
        {
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

        private void HandleSkill4InputStarted(InputAction.CallbackContext a_obj)
        {
            Debug.Log($"No Skill_4 yet");
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
        
            AssignedCharacter.MovementController.SetDirectionInput(moveInput.x * Vector3.right + moveInput.y * new Vector3(0f, 0.1f, 0.9f));
        }
    }
}
