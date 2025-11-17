using Game.Gameplay.PlayerCharacter.Movement.Data;
using Tools.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.MovementBehaviours
{
    public class DefaultMovementBehaviour : AMovementBehaviour
    {
        [field: SerializeField]
        public DefaultMovementDataAsset DefaultMovementDataAsset { get; private set; }
        
        private readonly NetworkVariable<Vector3> m_currentPlanarVelocity = 
            new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 CurrentPlanarVelocity => m_currentPlanarVelocity?.Value ?? Vector3.zero;




        protected override void FixedUpdateAsOwner()
        {
            if (!IsSpawned)
                return;
            if (Blackboard.IsBlocked)
                return;

            if (IsGroundedController.IsGrounded)
            {
                HandleGroundedMovement();
            }
            else
            {
                HandleInAirMovement();
            }
            
            Rigidbody.linearVelocity = CurrentPlanarVelocity.Flatten() + Vector3.up * FallingSpeedController.VerticalSpeed;
            Rigidbody.angularVelocity = Vector3.zero;
        }

        private void HandleGroundedMovement()
        {
            if (Blackboard.DirectionInput.sqrMagnitude > 0.3f * 0.3f)
            {
                var maxSpeed = DefaultMovementDataAsset.MaxMovementSpeed 
                               * DefaultMovementDataAsset.MaxSpeedFactorAccordingToInput.Evaluate(Blackboard.DirectionInput.magnitude);

                var newVelocitySpeed = m_currentPlanarVelocity.Value.magnitude + DefaultMovementDataAsset.Acceleration * Time.deltaTime;
                if (newVelocitySpeed > maxSpeed)
                {
                    newVelocitySpeed = maxSpeed;
                }

                var newVelocity = Blackboard.DirectionInput.normalized * newVelocitySpeed;
                m_currentPlanarVelocity.Value = newVelocity;
            }
            else
            {
                var decelerationDelta = DefaultMovementDataAsset.Deceleration * Time.deltaTime;
                if (m_currentPlanarVelocity.Value.magnitude < decelerationDelta)
                {
                    m_currentPlanarVelocity.Value = Vector3.zero;
                }
                else
                {
                    m_currentPlanarVelocity.Value -= m_currentPlanarVelocity.Value.normalized * decelerationDelta;
                }
            }

        }
        
        private void HandleInAirMovement()
        {
            
        }

        private float m_lastJumpTime;
        protected override void HandleJumpStarted_OwnerCalled(PlayerCharacterMovementBlackBoard a_obj)
        {
            if (Blackboard.IsBlocked)
                return;
            if (!IsGroundedController.IsGrounded)
                return;
            if (Time.time - m_lastJumpTime < 1f)
                return;
            
            FallingSpeedController.SetFallingSpeed_ForOwner(DefaultMovementDataAsset.JumpVelocity);
            m_lastJumpTime = Time.time;
        }

        protected override void HandleBlockActivated_OwnerCalled(PlayerCharacterMovementBlackBoard a_blackboard)
        {
            m_currentPlanarVelocity.Value = default;
            
            Rigidbody.linearVelocity = Vector3.down;
            Rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
