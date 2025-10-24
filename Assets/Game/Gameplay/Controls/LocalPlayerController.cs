using System;
using Game.Playground.Controls;
using Game.Playground.PlayerCharacter;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerController : MonoBehaviour
{
    public static LocalPlayerController Instance { get; private set; }
    
    private PlayerActions m_actions;

    private void Awake()
    {
        Instance = this;
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_actions = new PlayerActions();
        m_actions.Enable();

        m_actions.Movement.Jump.started += HandleJumpInputStarted;
        m_actions.Movement.Jump.canceled += HandleJumpInputCanceled;
        m_actions.Movement.Crouch.started += HandleCrouchInputStarted;
        m_actions.Movement.Crouch.canceled += HandleCrouchInputCanceled;
    }

    public PlayerCharacter AssignedCharacter { get; private set; }

    public event Action<LocalPlayerController> OnCharacterAssigned;
    public void AssignCharacter(PlayerCharacter a_character)
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

    private void Update()
    {
        if (m_actions == null) 
            return;
        /*if (!Application.isFocused)
            return;*/
        if (!AssignedCharacter)
            return;

        var moveInput = m_actions.Movement.Move.ReadValue<Vector2>();
        var lookAroundInput = m_actions.Camera.LookAround.ReadValue<Vector2>();
        
        AssignedCharacter.MovementController.SetDirectionInput(moveInput.x * Vector3.right + moveInput.y * new Vector3(0f, 0.1f, 0.9f));
    }
}
