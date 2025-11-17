using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public class PlayerCharacterMovementBlackBoard : NetworkBehaviour
    {
        private readonly NetworkVariable<bool> m_isBlocked =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);
        
        public bool IsBlocked => m_isBlocked?.Value ?? false;
        
        #region Blocking

        public event Action<PlayerCharacterMovementBlackBoard> OnBlockActivated_OwnerCalled;
        public event Action<PlayerCharacterMovementBlackBoard> OnBlockDeactivated_OwnerCalled;
        public event Action<PlayerCharacterMovementBlackBoard> OnBlockActivated_ServerCalled;
        public event Action<PlayerCharacterMovementBlackBoard> OnBlockDeactivated_ServerCalled;

        private readonly List<IPlayerCharacterMovementControllerBlocker> m_blockers = new();

        public void AddBlocker_ForServer(IPlayerCharacterMovementControllerBlocker a_blocker)
        {
            if (m_blockers.Contains(a_blocker))
                return;
            
            m_blockers.Add(a_blocker);
            UpdateBlockedState_Server();
        }

        public void RemoveBlocker_ForServer(IPlayerCharacterMovementControllerBlocker a_blocker)
        {
            m_blockers.RemoveAll(a_b => a_b == a_blocker);
            UpdateBlockedState_Server();
        }

        private void UpdateBlockedState_Server()
        {
            if (m_blockers.Count == 0 && m_isBlocked.Value)
            {
                m_isBlocked.Value = false; 
                
                OnBlockDeactivated_ServerCalled?.Invoke(this);
                HandleBlockDeactivated_OwnerRpc();
            }
            else if (m_blockers.Count > 0 && !m_isBlocked.Value)
            {
                m_isBlocked.Value = true;
                
                OnBlockActivated_ServerCalled?.Invoke(this);
                HandleBlockActivated_OwnerRpc();
            }
        }
        
        [Rpc(SendTo.Owner)]
        private void HandleBlockDeactivated_OwnerRpc()
        {
            OnBlockDeactivated_OwnerCalled?.Invoke(this);
        }
        
        [Rpc(SendTo.Owner)]
        private void HandleBlockActivated_OwnerRpc()
        {
            m_directionInput.Value = Vector2.zero;

            
            OnBlockActivated_OwnerCalled?.Invoke(this);
        }
        #endregion
        
                
        private readonly NetworkVariable<Vector3> m_directionInput = 
            new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 DirectionInput => m_directionInput?.Value ?? Vector3.zero;
        
        public void SetDirectionInput(Vector3 a_directionInput)
        {
            if (!IsSpawned)
                return;
            if (!IsOwner)
                return;
            if (IsBlocked)
                return;
            
/*
            Debug.Log($"Direction Input: {a_directionInput} | Old direction input : {DirectionInput}");
*/
            m_directionInput.Value = a_directionInput;
        }

        public event Action<PlayerCharacterMovementBlackBoard> OnJumpStarted_OwnerCalled;
        public event Action<PlayerCharacterMovementBlackBoard> OnJumpStopped_OwnerCalled;
        
        public void StartJump()
        {
            OnJumpStarted_OwnerCalled?.Invoke(this);
        }

        public void StopJump()
        {
            OnJumpStopped_OwnerCalled?.Invoke(this);
        }
    }
}