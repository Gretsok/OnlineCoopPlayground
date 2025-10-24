using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Playground.CharactersManagement
{
    public class PlayersCharactersManager : NetworkBehaviour
    {
        [SerializeField]
        private PlayerCharacter.PlayerCharacter m_characterPrefab;
        
        private Dictionary<ulong, PlayerCharacter.PlayerCharacter> m_characters = new();
        
        private NetworkManager m_networkManager;
        
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            m_networkManager = NetworkManager.Singleton;

            if (!m_networkManager.IsServer)
                return;

            HandleServerStarted();
            m_networkManager.OnClientConnectedCallback += HandleClientConnected;
            m_networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        private void HandleServerStarted()
        {
            var enumerator = m_networkManager.ConnectedClients.GetEnumerator();

            Debug.Log($"[PlayersCharactersManager] OnServerStarted : Existing client count is {m_networkManager.ConnectedClients.Count}");
            
            while (enumerator.MoveNext())
            {
                var client = enumerator.Current;
                if (m_characters.ContainsKey(client.Key))
                    continue;
                
                Debug.Log($"[PlayersCharactersManager] Adding character with Client ID {client.Key}");
                
                var character = Instantiate(m_characterPrefab, transform.position, Quaternion.identity);
                character.NetworkObject.SpawnWithOwnership(client.Key);
                m_characters.Add(client.Key, character);
            }
        }
    
        private void HandleClientConnected(ulong a_clientID)
        {
            Debug.Log($"[PlayersCharactersManager] OnClientConnected : Client ID {a_clientID} connected");
            if (m_characters.ContainsKey(a_clientID))
                return;
                
            Debug.Log($"[PlayersCharactersManager] Adding character with Client ID {a_clientID}");

            var character = Instantiate(m_characterPrefab, transform.position, Quaternion.identity);
            character.NetworkObject.SpawnWithOwnership(a_clientID);
            m_characters.Add(a_clientID, character);
        }
    
        private void HandleClientDisconnected(ulong a_clientID)
        {
            Debug.Log($"[PlayersCharactersManager] OnClientDisconnected : Client ID {a_clientID} disconnected");

            if (!m_characters.ContainsKey(a_clientID))
                return;
            
            Debug.Log($"[PlayersCharactersManager] Removing character with Client ID {a_clientID}");

            var character = m_characters[a_clientID];
            character.NetworkObject.Despawn();
        }
    }
}
