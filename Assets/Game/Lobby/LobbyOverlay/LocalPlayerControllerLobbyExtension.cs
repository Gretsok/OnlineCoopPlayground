using System;
using Game.Gameplay.Controls;
using Tools.UIManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Lobby.LobbyOverlay
{
    public class LocalPlayerControllerLobbyExtension : MonoBehaviour
    {
        private LocalPlayerController m_localPlayerController;
        private void Awake()
        {
            m_localPlayerController = GetComponent<LocalPlayerController>();

            m_localPlayerController.OnInputActionsInitialized += HandleInputActionsInitialized;
        }

        private void OnDestroy()
        {
            m_localPlayerController.OnInputActionsInitialized -= HandleInputActionsInitialized;
        }

        private void HandleInputActionsInitialized(LocalPlayerController a_localPlayerController)
        {
            a_localPlayerController.Actions.Lobby.ToggleLobbyMenu.started += HandleToggleLobbyMenuStarted;
        }

        private void HandleToggleLobbyMenuStarted(InputAction.CallbackContext a_obj)
        {
            var panel = UIManager.Instance.GetPanel<LobbyPanel>();
            panel.Toggle();
        }
    }
}
