using System;
using Game.Lobby;
using Netcode.Transports.Facepunch;
using Steamworks;
using TMPro;
using Tools.UIManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.Orchestration.Connecting.SteamConnection
{
    public class SteamConnectionWidgetsContainer : PanelExtensionHandler
    {
        private NetworkManager m_networkManager;
        
        
        [Header("Connection Panel Handling")]
        [SerializeField]
        private Button m_hostButton;

        [SerializeField]
        private Button m_clientButton;
        
        [SerializeField]
        private TMP_InputField m_targetSteamIdField;

        public Action OnConnectionToGameStarted;
        
        protected override void HandlePanelShown(Panel a_panel)
        {
            base.HandlePanelShown(a_panel);
            m_networkManager = NetworkManager.Singleton;

            var targetSteamId = m_networkManager.GetComponent<FacepunchTransport>().targetSteamId;
            m_targetSteamIdField.text = targetSteamId.ToString();
            
            m_clientButton.onClick.AddListener(HandleClientButtonClicked);
            m_hostButton.onClick.AddListener(HandleHostButtonClicked);
        }

        protected override void HandlePanelHidden(Panel a_panel)
        {
            base.HandlePanelHidden(a_panel);
            m_clientButton.onClick.RemoveListener(HandleClientButtonClicked);
            m_hostButton.onClick.RemoveListener(HandleHostButtonClicked);
        }

        private void UpdateConnectionData(bool a_asHost)
        {
            var transport = m_networkManager.GetComponent<FacepunchTransport>();
            m_networkManager.NetworkConfig.NetworkTransport = transport;
            
            if (ulong.TryParse(m_targetSteamIdField.text, out ulong lobbySteamId))
            {
                var lobbyManager = LobbyManager.Instance;
                if (a_asHost)
                    lobbyManager.CreateLobby();
                else
                    lobbyManager.JoinLobby(lobbySteamId);
                
                transport.targetSteamId = lobbyManager.Lobby.Owner.Id;
            }
            else
            {
                Debug.LogError($"Error when parsing lobby id.");
            }
        }
        
        private void HandleClientButtonClicked()
        {
            UpdateConnectionData(false);
            if (!m_networkManager.StartClient())
            {
                Debug.LogError($"Could not start client: {m_networkManager.GetComponent<FacepunchTransport>().targetSteamId}");
                return;
            }

            Debug.Log($"Client button clicked | Target Steam ID: {m_targetSteamIdField.text}");
            
            MainMenuOrchestrator.Instance.GoToConnectingScreen();
            OnConnectionToGameStarted?.Invoke();
        }

        private void HandleHostButtonClicked()
        {
            UpdateConnectionData(true);
            if (!m_networkManager.StartHost())
            {
                Debug.LogError($"Could not start host: {m_networkManager.GetComponent<FacepunchTransport>().targetSteamId}");
                return;
            }
            
            Debug.Log($"Host button clicked | Target Steam ID: {m_targetSteamIdField.text}");
            
            MainMenuOrchestrator.Instance.GoToConnectingScreen();
            OnConnectionToGameStarted?.Invoke();
        }
    }
}
