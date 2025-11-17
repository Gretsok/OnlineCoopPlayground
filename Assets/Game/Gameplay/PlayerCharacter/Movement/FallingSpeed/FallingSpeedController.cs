using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public class FallingSpeedController : NetworkBehaviour
    {
        [SerializeField]
        private float m_defaultGravity = 15f;
        
        private IsGroundedController m_isGroundedController;
        private PlayerCharacterMovementBlackBoard m_movementBlackBoard;

        public void SetDependencies(IsGroundedController a_isGroundedController, PlayerCharacterMovementBlackBoard a_movementBlackBoard)
        {
            m_isGroundedController = a_isGroundedController;
            m_movementBlackBoard = a_movementBlackBoard;
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            if (!IsOwner)
                return;
            
            m_movementBlackBoard.OnJumpStarted_OwnerCalled += HandleJumpStarted_OwnerCalled;
        }


        private readonly NetworkVariable<float> m_verticalSpeed = 
            new NetworkVariable<float>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);
        public float VerticalSpeed => m_verticalSpeed?.Value ?? 0;

        private float m_lastJumpTime;
        private void HandleJumpStarted_OwnerCalled(PlayerCharacterMovementBlackBoard a_obj)
        {
            m_lastJumpTime = Time.time;
        }

        public void SetFallingSpeed_ForOwner(float a_speed)
        {
            if (!IsOwner)
                return;
            
            m_verticalSpeed.Value = a_speed;
        }
        
        protected void FixedUpdate()
        {
            if (!IsOwner)
                return;
            if (!IsSpawned)
                return;
            if (m_movementBlackBoard.IsBlocked)
                return;

            if (m_isGroundedController.IsGrounded)
            {
                if (Time.time - m_lastJumpTime > 1f)
                    m_verticalSpeed.Value = -1f;
            }
            else
            {
                m_verticalSpeed.Value -= m_defaultGravity * Time.deltaTime;
            }
        }
    }
}
