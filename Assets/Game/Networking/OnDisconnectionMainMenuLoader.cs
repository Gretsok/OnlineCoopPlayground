using Game.Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Networking
{
    [RequireComponent(typeof(NetworkManager))]
    public class OnDisconnectionMainMenuLoader : MonoBehaviour
    {
        private NetworkManager m_networkManager;

        private void Awake()
        {
            m_networkManager = GetComponent<NetworkManager>();
            m_networkManager.OnClientDisconnectCallback += HandleClientDisconnectCallback;
        }

        private void HandleClientDisconnectCallback(ulong a_obj)
        {
            var isPlayerDisconnecting = m_networkManager.LocalClientId == a_obj;
            var isServer = m_networkManager.IsServer;

            if (isPlayerDisconnecting)
                Debug.Log($"[DISCONNECTION] Reason: {m_networkManager.DisconnectReason}");
            else if (isServer)
                Debug.Log($"[REMOTE-CLIENT-DISCONNECTION] Reason: {m_networkManager.DisconnectReason}");

            if (!isPlayerDisconnecting)
                return;
            
            SceneManager.LoadSceneAsync("MainMenu");
            LobbyManager.Instance.LeaveLobby();
        }
    }
}
