using System;
using System.Collections.Generic;
using Game.Gameplay.PlayerCharacter.MotorImplementations.Default;
using Game.Gameplay.VehiclesSystem;
using Game.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Gameplay.CharactersManagement
{
    public class PlayersCharactersManager : NetworkBehaviour
    {
        public enum ECharacterType
        {
            Default = 0,
            Vehicle = 1
        }
        
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
        
        [SerializeField]
        private DefaultPlayerMotor m_defaultMotorPrefab;
        [SerializeField]
        private VehiclePlayerMotor m_vehicleMotorPrefab;
        
        private Dictionary<ulong, PlayerCharacter.APlayerMotor> m_characters = new();
        

        
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
        
        private PlayerCharacter.APlayerMotor InstantiateAndSpawnCharacterFor(ulong a_clientID, ECharacterType a_characterType = ECharacterType.Default)
        {
            if (m_characters.ContainsKey(a_clientID))
                return m_characters[a_clientID];
                
            Debug.Log($"[PlayersCharactersManager] Adding character with Client ID {a_clientID}");

            var character = Instantiate(m_defaultMotorPrefab, transform.position, Quaternion.identity);
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
