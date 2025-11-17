using Game.Gameplay.Controls;
using Game.Gameplay.PlayerCharacter.Implementations.Default;
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

            m_localPlayerController.DefaultLocalPlayerInputProcessor.OnInputActionsInitialized += HandleInputActionsInitialized;
        }

        private void OnDestroy()
        {
            m_localPlayerController.DefaultLocalPlayerInputProcessor.Actions.Lobby.ToggleLobbyMenu.started -= HandleToggleLobbyMenuStarted;

            m_localPlayerController.DefaultLocalPlayerInputProcessor.OnInputActionsInitialized -= HandleInputActionsInitialized;
        }

        private void HandleInputActionsInitialized(DefaultLocalPlayerInputProcessor a_defaultLocalPlayerInputProcessor)
        {
            a_defaultLocalPlayerInputProcessor.Actions.Lobby.ToggleLobbyMenu.started += HandleToggleLobbyMenuStarted;
        }

        private void HandleToggleLobbyMenuStarted(InputAction.CallbackContext a_obj)
        {
            var panel = UIManager.Instance.GetPanel<LobbyPanel>();
            panel.Toggle();
        }
    }
}
