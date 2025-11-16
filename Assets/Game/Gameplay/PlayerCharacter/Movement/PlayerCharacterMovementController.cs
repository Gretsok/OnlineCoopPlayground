using System.Collections.Generic;
using Game.Gameplay.PlayerCharacter.Movement.Data;
using Tools.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public class PlayerCharacterMovementController : NetworkBehaviour
    {
        [field: SerializeField]
        public MovementDataAsset MovementDataAsset { get; private set; }
        
        private Rigidbody m_rigidbody;
        private IsGroundedController m_isGroundedController;
        
        public void SetDependencies(Rigidbody a_rigidbody, IsGroundedController a_isGroundedController)
        {
            m_rigidbody = a_rigidbody;
            m_isGroundedController = a_isGroundedController;
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
            m_currentPlanarVelocity.Value = default;
            
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
        private readonly NetworkVariable<Vector3> m_currentPlanarVelocity = 
            new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 CurrentPlanarVelocity => m_currentPlanarVelocity?.Value ?? Vector3.zero;

        private readonly NetworkVariable<float> m_verticalSpeed = 
            new NetworkVariable<float>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public float VerticalSpeed => m_verticalSpeed?.Value ?? 0;
        
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

        private void FixedUpdate()
        {
            if (!IsSpawned)
                return;
            if (!IsOwner)
                return;
            if (m_isBlocked.Value)
                return;

            if (m_isGroundedController.IsGrounded)
            {
                HandleGroundedMovement();
            }
            else
            {
                HandleInAirMovement();
            }
            
            m_rigidbody.linearVelocity = CurrentPlanarVelocity.Flatten() + Vector3.up * VerticalSpeed;
            m_rigidbody.angularVelocity = Vector3.zero;
        }

        private void HandleGroundedMovement()
        {
            if (m_directionInput.Value.sqrMagnitude > 0.3f * 0.3f)
            {
                var maxSpeed = MovementDataAsset.MaxMovementSpeed 
                               * MovementDataAsset.MaxSpeedFactorAccordingToInput.Evaluate(m_directionInput.Value.magnitude);

                var newVelocitySpeed = m_currentPlanarVelocity.Value.magnitude + MovementDataAsset.Acceleration * Time.deltaTime;
                if (newVelocitySpeed > maxSpeed)
                {
                    newVelocitySpeed = maxSpeed;
                }

                var newVelocity = m_directionInput.Value.normalized * newVelocitySpeed;
                m_currentPlanarVelocity.Value = newVelocity;
            }
            else
            {
                var decelerationDelta = MovementDataAsset.Deceleration * Time.deltaTime;
                if (m_currentPlanarVelocity.Value.magnitude < decelerationDelta)
                {
                    m_currentPlanarVelocity.Value = Vector3.zero;
                }
                else
                {
                    m_currentPlanarVelocity.Value -= m_currentPlanarVelocity.Value.normalized * decelerationDelta;
                }
            }
            if (Time.time - m_lastJumpTime > 1f)
                m_verticalSpeed.Value = -1f;
        }
        
        private void HandleInAirMovement()
        {
            m_verticalSpeed.Value -= MovementDataAsset.GravityAcceleration * Time.deltaTime;
        }

        private float m_lastJumpTime;
        public void Jump()
        {
            if (!IsOwner)
                return;
            if (m_isBlocked.Value)
                return;
            if (!m_isGroundedController.IsGrounded)
                return;
            if (Time.time - m_lastJumpTime < 1f)
                return;
            
            m_verticalSpeed.Value = MovementDataAsset.JumpVelocity;
            m_lastJumpTime = Time.time;
        }
    }
}
