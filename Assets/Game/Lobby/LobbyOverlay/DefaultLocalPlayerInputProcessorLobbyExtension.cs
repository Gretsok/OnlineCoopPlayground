using Game.Gameplay.PlayerCharacter.MotorImplementations.Default;
using Tools.UIManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Lobby.LobbyOverlay
{
    public class DefaultLocalPlayerInputProcessorLobbyExtension : MonoBehaviour
    {
        private DefaultLocalPlayerInputProcessor m_inputProcessor;
        private void Awake()
        {
            m_inputProcessor = GetComponent<DefaultLocalPlayerInputProcessor>();

            m_inputProcessor.OnInputActionsInitialized += HandleInputActionsInitialized;
        }

        private void OnDestroy()
        {
            m_inputProcessor.OnInputActionsInitialized -= HandleInputActionsInitialized;
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
