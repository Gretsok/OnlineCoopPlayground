using System.Collections.Generic;
using Game.Gameplay.PlayerCharacter.Movement.Data;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public class PlayerCharacterMovementController : NetworkBehaviour
    {
        [field: SerializeField]
        public MovementDataAsset MovementDataAsset { get; private set; }

        private Rigidbody m_rigidbody;
        
        public void SetDependencies(Rigidbody a_rigidbody)
        {
            m_rigidbody = a_rigidbody;
        }
        
        #region Blocking

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
            }
            else if (m_blockers.Count > 0 && !m_isBlocked.Value)
            {
                m_isBlocked.Value = true;
                
                StopMovement_OwnerRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void StopMovement_OwnerRpc()
        {
            m_directionInput.Value = Vector2.zero;
            m_currentVelocity.Value = default;
            
            m_rigidbody.linearVelocity = Vector3.down;
            m_rigidbody.angularVelocity = Vector3.zero;
        }

        private readonly NetworkVariable<bool> m_isBlocked =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);
        
        #endregion

        private readonly NetworkVariable<Vector3> m_directionInput = 
            new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 DirectionInput => m_directionInput?.Value ?? Vector3.zero;
        private readonly NetworkVariable<Vector3> m_currentVelocity = 
            new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 CurrentVelocity => m_currentVelocity?.Value ?? Vector3.zero;

        public void SetDirectionInput(Vector3 a_directionInput)
        {
            if (!IsSpawned)
                return;
            if (!IsOwner)
                return;
            
            
            if (m_isBlocked.Value)
                return;
            
/*
            Debug.Log($"Direction Input: {a_directionInput} | Old direction input : {DirectionInput}");
*/
            m_directionInput.Value = a_directionInput;
        }

        private void Update()
        {
            if (!IsSpawned)
                return;
            if (!IsOwner)
                return;
            
            
            if (m_isBlocked.Value)
                return;

            if (m_directionInput.Value.sqrMagnitude > 0.3f * 0.3f)
            {
                var maxSpeed = MovementDataAsset.MaxMovementSpeed 
                               * MovementDataAsset.MaxSpeedFactorAccordingToInput.Evaluate(m_directionInput.Value.magnitude);
                m_rigidbody.maxLinearVelocity = maxSpeed;

                var newVelocitySpeed = m_currentVelocity.Value.magnitude + MovementDataAsset.Acceleration * Time.deltaTime;
                if (newVelocitySpeed > maxSpeed)
                {
                    newVelocitySpeed = maxSpeed;
                }

                var newVelocity = m_directionInput.Value.normalized * newVelocitySpeed;
                m_currentVelocity.Value = newVelocity;
                
            }
            else
            {
                var decelerationDelta = MovementDataAsset.Deceleration * Time.deltaTime;
                if (m_currentVelocity.Value.magnitude < decelerationDelta)
                {
                    m_currentVelocity.Value = Vector3.zero;
                }
                else
                {
                    m_currentVelocity.Value -= m_currentVelocity.Value.normalized * decelerationDelta;
                }
            }

            m_rigidbody.linearVelocity = m_currentVelocity.Value + Vector3.down;
            m_rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
