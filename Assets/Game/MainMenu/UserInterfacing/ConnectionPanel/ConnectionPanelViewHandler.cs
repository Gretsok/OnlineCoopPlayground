using System;
using Game.MainMenu.Orchestration;
using TMPro;
using Tools.UIManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.UserInterfacing.ConnectionPanel
{
    public class ConnectionPanelViewHandler : PanelExtensionHandler
    {
        private NetworkManager m_networkManager;
        
        
        [Header("Connection Panel Handling")]
        [SerializeField]
        private Button m_hostButton;

        [SerializeField]
        private Button m_clientButton;
        
        [SerializeField]
        private TMP_InputField m_addressField;
        [SerializeField]
        private TMP_InputField m_portField;

        public Action OnConnectionToGameStarted;
        
        protected override void HandlePanelShown(Panel a_panel)
        {
            base.HandlePanelShown(a_panel);
            m_networkManager = NetworkManager.Singleton;

            var connectionData = m_networkManager.GetComponent<UnityTransport>().ConnectionData;
            m_addressField.text = connectionData.Address;
            m_portField.text = connectionData.Port.ToString();
            
            m_clientButton.onClick.AddListener(HandleClientButtonClicked);
            m_hostButton.onClick.AddListener(HandleHostButtonClicked);
        }

        protected override void HandlePanelHidden(Panel a_panel)
        {
            base.HandlePanelHidden(a_panel);
            m_clientButton.onClick.RemoveListener(HandleClientButtonClicked);
            m_hostButton.onClick.RemoveListener(HandleHostButtonClicked);
        }

        private void UpdateConnectionData()
        {
            var connectionData = m_networkManager.GetComponent<UnityTransport>().ConnectionData;
            connectionData.Address = m_addressField.text;
            if (ushort.TryParse(m_portField.text, out ushort portToUse))
            {
                connectionData.Port = portToUse;
            }
            m_networkManager.GetComponent<UnityTransport>().ConnectionData = connectionData;
        }
        
        private void HandleClientButtonClicked()
        {
            UpdateConnectionData();
            if (!m_networkManager.StartClient())
            {
                Debug.LogError($"Could not start client: {m_networkManager.GetComponent<UnityTransport>().ConnectionData}");
                return;
            }

            Debug.Log($"Client button clicked | Address: {m_addressField.text} & Port: {m_portField.text}");
            
            MainMenuOrchestrator.Instance.GoToConnectingScreen();
            OnConnectionToGameStarted?.Invoke();
        }

        private void HandleHostButtonClicked()
        {
            UpdateConnectionData();
            if (!m_networkManager.StartHost())
            {
                Debug.LogError($"Could not start host: {m_networkManager.GetComponent<UnityTransport>().ConnectionData}");
                return;
            }
            
            Debug.Log($"Host button clicked | Address: {m_addressField.text} & Port: {m_portField.text}");
            
            MainMenuOrchestrator.Instance.GoToConnectingScreen();
            OnConnectionToGameStarted?.Invoke();
        }
    }
}
