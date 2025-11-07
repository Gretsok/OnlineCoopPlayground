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
            Debug.Log($"[DISCONNECTION] Reason: {m_networkManager.DisconnectReason}");
            SceneManager.LoadSceneAsync("MainMenu");
            LobbyManager.Instance.LeaveLobby();
        }
    }
}
