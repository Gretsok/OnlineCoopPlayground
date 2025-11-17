using System;
using System.Collections.Generic;
using Game.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Gameplay.CharactersManagement
{
    public class PlayersCharactersManager : NetworkBehaviour
    {
        public static PlayersCharactersManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }


        [FormerlySerializedAs("m_characterPrefab")]
        [SerializeField]
        private PlayerCharacter.DefaultPlayerMotor m_motorPrefab;
        
        private Dictionary<ulong, PlayerCharacter.DefaultPlayerMotor> m_characters = new();
        
        private NetworkManager m_networkManager;
        
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            m_networkManager = NetworkManager.Singleton;
/*
            if (!m_networkManager.IsServer)
                return;

            HandleServerStarted();
            m_networkManager.OnClientConnectedCallback += HandleClientConnected;
            m_networkManager.OnClientDisconnectCallback += HandleClientDisconnected;*/
        }
        
    /*    private void HandleServerStarted()
        {
            return;
            var enumerator = m_networkManager.ConnectedClients.GetEnumerator();

            Debug.Log($"[PlayersCharactersManager] OnServerStarted : Existing client count is {m_networkManager.ConnectedClients.Count}");
            
            while (enumerator.MoveNext())
            {
                var client = enumerator.Current;
                if (m_characters.ContainsKey(client.Key))
                    continue;
                
                Debug.Log($"[PlayersCharactersManager] Adding character with Client ID {client.Key}");
                
                var character = Instantiate(m_motorPrefab, transform.position, Quaternion.identity);
                character.NetworkObject.SpawnWithOwnership(client.Key);
                m_characters.Add(client.Key, character);
            }
        }
        
        private void HandleClientConnected(ulong a_clientID)
        {
            return;
            Debug.Log($"[PlayersCharactersManager] OnClientConnected : Client ID {a_clientID} connected");
            InstantiateAndSpawnCharacterFor(a_clientID);
        }
        
        private void HandleClientDisconnected(ulong a_clientID)
        {
            return;
            if (!m_networkManager.IsListening && !m_networkManager.IsConnectedClient)
                return;

            DespawnAndDestroyCharacterOf(a_clientID);
        }*/

        private event Action<PlayerCharacter.APlayerMotor> m_tempResultCallback;
        public void RetrieveCharacterFor_ForOwner(AbstractConnectedClientObject a_client, Action<PlayerCharacter.APlayerMotor> a_resultCallback)
        {
            m_tempResultCallback += a_resultCallback;
            RetrieveCharacterFor_ServerRpc(a_client);
        }

        [Rpc(SendTo.Server)]
        private void RetrieveCharacterFor_ServerRpc(NetworkBehaviourReference a_clientReference)
        {
            if (!a_clientReference.TryGet(out AbstractConnectedClientObject client))
            {
                Debug.LogError($"[SERVER] Cannot retrieve client from client reference {a_clientReference}");
                return;
            }
            
            var character = InstantiateAndSpawnCharacterFor(client.OwnerClientId);
            SendBackCharacter_OwnerRpc(character, RpcTarget.Single(client.OwnerClientId, RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SendBackCharacter_OwnerRpc(NetworkBehaviourReference a_characterReference, RpcParams a_rpcParams = default)
        {
            if (!a_characterReference.TryGet(out PlayerCharacter.APlayerMotor character))
            {
                Debug.LogError($"[OWNER] Cannot retrieve character from character reference {a_characterReference}");
                return;
            }
            
            m_tempResultCallback?.Invoke(character);
            m_tempResultCallback = null;
        }
    


        private PlayerCharacter.DefaultPlayerMotor InstantiateAndSpawnCharacterFor(ulong a_clientID)
        {
            if (m_characters.ContainsKey(a_clientID))
                return m_characters[a_clientID];
                
            Debug.Log($"[PlayersCharactersManager] Adding character with Client ID {a_clientID}");

            var character = Instantiate(m_motorPrefab, transform.position, Quaternion.identity);
            character.NetworkObject.SpawnWithOwnership(a_clientID);
            m_characters.Add(a_clientID, character);
            return character;
        }


        public void DeleteCharacterFor_ForOwner(AbstractConnectedClientObject a_client)
        {
            if (!a_client.IsOwner)
                return;
            
            DeleteCharacterFor_ServerRpc(a_client.OwnerClientId);
        }

        [Rpc(SendTo.Server)]
        private void DeleteCharacterFor_ServerRpc(ulong a_clientID)
        {
            DespawnAndDestroyCharacterOf(a_clientID);
        }
        
        private void DespawnAndDestroyCharacterOf(ulong a_clientID)
        {
            Debug.Log($"[PlayersCharactersManager] OnClientDisconnected : Client ID {a_clientID} disconnected");

            if (!m_characters.ContainsKey(a_clientID))
                return;
            
            Debug.Log($"[PlayersCharactersManager] Removing character with Client ID {a_clientID}");

            var character = m_characters[a_clientID];
            m_characters.Remove(a_clientID);
            Destroy(character?.NetworkObject?.gameObject);
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            if (!IsServer)
                return;
/*
            m_networkManager.OnClientConnectedCallback -= HandleClientConnected;
            m_networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
            */
            var enumerator = m_characters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var kvp = enumerator.Current;
                
                if(!kvp.Value || !kvp.Value.NetworkObject)
                    continue;
                
                if (kvp.Value.NetworkObject.IsSpawned)
                    kvp.Value.NetworkObject.Despawn();
                else
                    Destroy(kvp.Value.NetworkObject.gameObject);
            }
            
            m_characters.Clear();
        }
    }
}
