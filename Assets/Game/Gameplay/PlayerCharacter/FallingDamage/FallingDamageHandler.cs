using System;
using System.Collections.Generic;
using Game.Gameplay.GameplayInteractionsSystems.HealthHandling;
using Game.Gameplay.PlayerCharacter.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.FallingDamage
{
    public class FallingDamageHandler : NetworkBehaviour
    {
        private IsGroundedController m_isGroundedController;
        private PlayerCharacterMovementController m_movementController;
        private HealthController m_healthController;

        [System.Serializable]
        public struct SFallingDamageInfo
        {
            public float MinimumFallingSpeed;
            public int DamageToDeal;
        }
        
        [SerializeField]
        private List<SFallingDamageInfo> m_fallingDamageInfos = new();

        public event Action<FallingDamageHandler, int> OnFallingDamageTaken_ServerCalled;
        public event Action<FallingDamageHandler, int> OnFallingDamageTaken_ClientsCalled;

        private void Start()
        {
            m_fallingDamageInfos
                .Sort((a_info1, a_info2) => a_info1.MinimumFallingSpeed.CompareTo(a_info2.MinimumFallingSpeed));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // It changes every we type a digit, very annoying... We should find another to ensure it remains in the right order.
            // => We do it in the start!
            /*
            m_fallingDamageInfos
                .Sort((a_info1, a_info2) => a_info1.MinimumFallingSpeed.CompareTo(a_info2.MinimumFallingSpeed));
                */
        }
#endif

        public void SetDependencies(IsGroundedController a_isGroundedController, 
            PlayerCharacterMovementController a_movementController,
            HealthController a_healthController)
        {
            m_isGroundedController = a_isGroundedController;
            m_movementController = a_movementController;
            m_healthController = a_healthController;

            if (!IsServer)
                return;
            
            m_isGroundedController.OnGrounded_ServerCalled += HandleGrounded_ServerCalled;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (!IsServer)
                return;
            
            m_isGroundedController.OnGrounded_ServerCalled -= HandleGrounded_ServerCalled;
        }

        /// <summary>
        /// Avoid fall to be detected several times in a short span.
        /// </summary>
        private float m_damageHandlingCooldown = 1f;
        private float m_lastTimeDamageHandled = 0f;
        private void HandleGrounded_ServerCalled(IsGroundedController a_obj)
        {
            if (Time.time - m_lastTimeDamageHandled < m_damageHandlingCooldown)
                return;

            var fallingSpeed = -m_movementController.VerticalSpeed;
            for (int i = 0; i < m_fallingDamageInfos.Count; i++)
            {
                var info = m_fallingDamageInfos[i];

                if (i == m_fallingDamageInfos.Count - 1
                    && info.MinimumFallingSpeed < fallingSpeed)
                {
                    ApplyFallDamage(info, fallingSpeed);
                }
                
                if (info.MinimumFallingSpeed > fallingSpeed)
                {
                    if (i == 0)
                        break;

                    ApplyFallDamage(m_fallingDamageInfos[i - 1], fallingSpeed);
                    break;
                }
            }
        }

        private void ApplyFallDamage(SFallingDamageInfo a_info, float a_fallingSpeed)
        {
            var damageToDeal = a_info.DamageToDeal;
            m_healthController.TakeDamage_ForServer(damageToDeal);
            Debug.Log($"[SERVER] Fall damage of {a_info.DamageToDeal} applied to {m_movementController.OwnerClientId} for a falling speed of {a_fallingSpeed}");
                    
            OnFallingDamageTaken_ServerCalled?.Invoke(this, damageToDeal);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleFallingDamageTaken_ClientsRpc(int a_fallDamage)
        {
            OnFallingDamageTaken_ClientsCalled?.Invoke(this, a_fallDamage);
        }
    }
}
