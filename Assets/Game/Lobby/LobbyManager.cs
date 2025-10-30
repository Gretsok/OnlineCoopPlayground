using System;
using System.Collections;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

namespace Game.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        public Steamworks.Data.Lobby Lobby { get; private set; }

        public bool IsInLobby { get; private set; } = false;
        
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        
        
        #region Orchestration

        private NetworkManager m_networkManager;
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton);
            m_networkManager = NetworkManager.Singleton;
            
            m_networkManager.OnServerStopped += HandleServerStopped;
        }

        private void OnDestroy()
        {
            if (m_networkManager)
                m_networkManager.OnServerStopped -= HandleServerStopped;
        }

        private void HandleServerStopped(bool a_obj)
        {
            LeaveLobby();
        }
        
        #endregion
        
#region Lobby Management methods
        public async void CreateLobby()
        {
            try
            {
                if (IsInLobby)
                {
                    Debug.LogError("Cannot create lobby: Already In Lobby.");
                    return;
                }
                
                var lobbyRequest = await SteamMatchmaking.CreateLobbyAsync(4);
                Lobby = lobbyRequest.GetValueOrDefault();
                IsInLobby = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async void JoinLobby(SteamId a_steamID)
        {
            try
            {
                if (IsInLobby)
                {
                    Debug.LogError("Cannot join lobby: Already In Lobby");
                    return;
                }
                
                var lobbyRequest = await SteamMatchmaking.JoinLobbyAsync(a_steamID);
                Lobby = lobbyRequest.GetValueOrDefault();
                IsInLobby = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async void LeaveLobby()
        {
            try
            {
                if (!IsInLobby)
                {
                    Debug.LogError("Cannot leave lobby: Not In Lobby");
                    return;
                }

                Lobby.Leave();
                IsInLobby = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
#endregion

    }
}
