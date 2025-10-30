using System;
using Netcode.Transports.Facepunch;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Lobby.LobbyOverlay.MainActionsSection
{
    public class MainActionsWidgetsHandler : MonoBehaviour
    {
        [SerializeField]
        private Button m_startGameButton;
        [SerializeField]
        private Button m_openFriendListButton;
        [SerializeField]
        private Button m_leaveLobbyButton;

        private NetworkManager m_networkManager;
        
        private void Start()
        {
            m_networkManager = NetworkManager.Singleton;

            if (m_networkManager.IsServer)
            {
                m_startGameButton.onClick.AddListener(HandleStartGameButtonClicked);
            }
            else
            {
                m_startGameButton.gameObject.SetActive(false);
            }
            
            m_openFriendListButton.onClick.AddListener(HandleOpenFriendListButtonClicked);
            m_leaveLobbyButton.onClick.AddListener(HandleLeaveLobbyButtonClicked);
        }

        private void HandleStartGameButtonClicked()
        {
            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsServer)
                return;
            
            networkManager.SceneManager.LoadScene("PlaygroundScene", LoadSceneMode.Single);
            
            LobbyManager.Instance.Lobby.SetJoinable(false);
        }
        
        private void HandleOpenFriendListButtonClicked()
        {
            SteamFriends.OpenOverlay("Friends");
        }
        
        private void HandleLeaveLobbyButtonClicked()
        {
            m_networkManager.Shutdown();
        }
    }
}
