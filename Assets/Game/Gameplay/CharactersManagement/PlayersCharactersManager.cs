using System;
using System.Collections.Generic;
using Game.Gameplay.PlayerCharacter;
using Game.Gameplay.PlayerCharacter.CharacterImplementations;
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
        public enum EPlayerMotorType
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
        private PlayerCharacterPawn m_playerCharacterPawnPrefab;
        [Header("Player Motors")]
        [SerializeField]
        private DefaultPlayerMotor m_defaultMotorPrefab;
        [SerializeField]
        private VehiclePlayerMotor m_vehicleMotorPrefab;
        
        private Dictionary<ulong, APlayerMotor> m_motors = new();
        

        

        private event Action<APlayerMotor> m_tempResultAfterGetRequestCallback;
        public void RequestMotorFor_ForClients(ulong a_clientID,
            Action<APlayerMotor> a_resultCallback)
        {
            m_tempResultCallback = a_resultCallback;
            RequestMotorFor_ServerRpc(a_clientID);
        }

        [Rpc(SendTo.Server)]
        private void RequestMotorFor_ServerRpc(ulong a_clientID)
        {
            var motor = m_motors.ContainsKey(a_clientID) ? m_motors[a_clientID] : null;
            SendBackMotorAfterGetRequest_OwnerRpc(motor, RpcTarget.Single(a_clientID, RpcTargetUse.Temp));
        }
        
        [Rpc(SendTo.SpecifiedInParams)]
        private void SendBackMotorAfterGetRequest_OwnerRpc(NetworkBehaviourReference a_motorReference, RpcParams a_rpcParams = default)
        {
            if (!a_motorReference.TryGet(out APlayerMotor motor))
            {
                Debug.LogError($"[OWNER] Cannot retrieve motor from motor reference {a_motorReference}");
                return;
            }
            
            m_tempResultAfterGetRequestCallback?.Invoke(motor);
            m_tempResultAfterGetRequestCallback = null;
        }
        
        
        private event Action<APlayerMotor> m_tempResultCallback;

        public void ChangeMotorTypeFor_ForOwner(AbstractConnectedClientObject a_client,
            Action<APlayerMotor> a_resultCallback, EPlayerMotorType a_playerMotorType)
        {
            if (!a_client.IsOwner)
                return;
            
            m_tempResultCallback = a_resultCallback;
            ChangeMotorTypeFor_ServerRpc(a_client, a_playerMotorType);
        }
        
        public void ChangeMotorTypeFor_ForServer(AbstractConnectedClientObject a_client, EPlayerMotorType a_playerMotorType)
        {
            if (!a_client.IsOwner)
                return;
            
            ChangeMotorTypeFor_ServerRpc(a_client, a_playerMotorType);
        }

        [Rpc(SendTo.Server)]
        private void ChangeMotorTypeFor_ServerRpc(NetworkBehaviourReference a_clientReference,
            EPlayerMotorType a_playerMotorType)
        {
            if (!IsServer)
                return;
            
            if (!a_clientReference.TryGet(out AbstractConnectedClientObject client))
            {
                Debug.LogError($"[SERVER] Cannot retrieve client from client reference {a_clientReference}");
                return;
            }
            
            if (!m_motors.ContainsKey(client.OwnerClientId))
            {
                CreateMotorFor_ServerRpc(client,  a_playerMotorType);
            }
            else
            {
                var previousMotor = m_motors[client.OwnerClientId];
                
                var newMotor = InstantiateAndSpawnMotorFor(client.OwnerClientId, a_playerMotorType);
                newMotor.SetUpPawnInMotor_ForServer(previousMotor.PlayerCharacterPawn);
                m_motors[client.OwnerClientId] = newMotor;
                SendBackMotorAfterCreation_OwnerRpc(newMotor, RpcTarget.Single(client.OwnerClientId, RpcTargetUse.Temp));
                Destroy(previousMotor.gameObject);
            }
        }
        
        public void CreateMotorFor_ForOwner(AbstractConnectedClientObject a_client, Action<APlayerMotor> a_resultCallback, EPlayerMotorType a_playerMotorType)
        {
            if (!a_client.IsOwner)
                return;
            
            m_tempResultCallback += a_resultCallback;
            CreateMotorFor_ServerRpc(a_client, a_playerMotorType);
        }

        [Rpc(SendTo.Server)]
        private void CreateMotorFor_ServerRpc(NetworkBehaviourReference a_clientReference, EPlayerMotorType a_playerMotorType)
        {
            if (!a_clientReference.TryGet(out AbstractConnectedClientObject client))
            {
                Debug.LogError($"[SERVER] Cannot retrieve client from client reference {a_clientReference}");
                return;
            }
            var motor = InstantiateAndSpawnMotorFor(client.OwnerClientId, a_playerMotorType);
            motor.SetUpPawnInMotor_ForServer();

            if (m_motors.ContainsKey(client.OwnerClientId))
            {
                DeleteMotorFor_ServerRpc(client.OwnerClientId);
            }
            
            m_motors.Add(client.OwnerClientId, motor);
            
            SendBackMotorAfterCreation_OwnerRpc(motor, RpcTarget.Single(client.OwnerClientId, RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SendBackMotorAfterCreation_OwnerRpc(NetworkBehaviourReference a_motorReference, RpcParams a_rpcParams = default)
        {
            if (!a_motorReference.TryGet(out APlayerMotor motor))
            {
                Debug.LogError($"[OWNER] Cannot retrieve motor from motor reference {a_motorReference}");
                return;
            }
            
            m_tempResultCallback?.Invoke(motor);
            m_tempResultCallback = null;
        }
        
        private APlayerMotor InstantiateAndSpawnMotorFor(ulong a_clientID, EPlayerMotorType a_playerMotorType)
        {
                
            Debug.Log($"[PlayersCharactersManager] Adding motor with Client ID {a_clientID}");
            APlayerMotor motor = null;
            if (a_playerMotorType == EPlayerMotorType.Vehicle)
            {
                motor = Instantiate(m_vehicleMotorPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                motor = Instantiate(m_defaultMotorPrefab, transform.position, Quaternion.identity);
            }
            motor.NetworkObject.SpawnWithOwnership(a_clientID);

            return motor;
        }
        
        public void DeleteMotorFor_ForOwner(AbstractConnectedClientObject a_client)
        {
            if (!a_client.IsOwner)
                return;
            
            DeleteMotorFor_ServerRpc(a_client.OwnerClientId);
        }

        [Rpc(SendTo.Server)]
        private void DeleteMotorFor_ServerRpc(ulong a_clientID)
        {
            DespawnAndDestroyMotorOf(a_clientID);
        }
        
        private void DespawnAndDestroyMotorOf(ulong a_clientID)
        {
            Debug.Log($"[PlayersCharactersManager] OnClientDisconnected : Client ID {a_clientID} disconnected");

            if (!m_motors.ContainsKey(a_clientID))
                return;
            
            Debug.Log($"[PlayersCharactersManager] Removing motor with Client ID {a_clientID}");

            var motor = m_motors[a_clientID];
            m_motors.Remove(a_clientID);
            Destroy(motor?.NetworkObject?.gameObject);
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            if (!IsServer)
                return;

            var enumerator = m_motors.GetEnumerator();
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
            
            m_motors.Clear();
        }
    }
}
