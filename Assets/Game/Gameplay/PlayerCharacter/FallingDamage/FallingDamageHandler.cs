using System;
using System.Collections.Generic;
using Game.Gameplay.GameplayInteractionsSystems.HealthHandling;
using Game.Gameplay.PlayerCharacter.Movement;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.FallingDamage
{
    public class FallingDamageHandler : NetworkBehaviour
    {
        private IsGroundedController m_isGroundedController;
        private FallingSpeedController m_fallingSpeedController;
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

        public void SetDependencies(IsGroundedController a_isGroundedController, 
            FallingSpeedController a_fallingSpeedController,
            HealthController a_healthController)
        {
            m_isGroundedController = a_isGroundedController;
            m_fallingSpeedController = a_fallingSpeedController;
            m_healthController = a_healthController;

            if (!IsOwner)
                return;
            
            m_isGroundedController.OnGrounded_OwnerCalled += HandleGrounded_OwnerCalled;
        }
        
        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            if (!IsOwner)
                return;
            
            m_isGroundedController.OnGrounded_OwnerCalled -= HandleGrounded_OwnerCalled;
        }

        /// <summary>
        /// Avoid fall to be detected several times in a short span.
        /// </summary>
        private float m_damageHandlingCooldown = 1f;
        private float m_lastTimeDamageHandled = 0f;
        private void HandleGrounded_OwnerCalled(IsGroundedController a_obj)
        {
            if (Time.time - m_lastTimeDamageHandled < m_damageHandlingCooldown)
                return;

            m_lastTimeDamageHandled = Time.time;
            var fallingSpeed = -m_fallingSpeedController.VerticalSpeed;
            HandleGrounded_ServerRpc(fallingSpeed);
        }

        [Rpc(SendTo.Server, RequireOwnership = true)]
        private void HandleGrounded_ServerRpc(float a_fallingSpeed)
        {
            for (int i = 0; i < m_fallingDamageInfos.Count; i++)
            {
                var info = m_fallingDamageInfos[i];

                if (i == m_fallingDamageInfos.Count - 1
                    && info.MinimumFallingSpeed < a_fallingSpeed)
                {
                    ApplyFallDamage(info, a_fallingSpeed);
                    return;
                }
                
                if (info.MinimumFallingSpeed > a_fallingSpeed)
                {
                    if (i == 0)
                        return;

                    ApplyFallDamage(m_fallingDamageInfos[i - 1], a_fallingSpeed);
                    return;
                }
            }
            
            Debug.Log($"[SERVER] Not enough falling speed to receive damage : {a_fallingSpeed} m/s.");
        }
        
        private void ApplyFallDamage(SFallingDamageInfo a_info, float a_fallingSpeed)
        {
            var damageToDeal = a_info.DamageToDeal;
            m_healthController.TakeDamage_ForServer(damageToDeal);
            Debug.Log($"[SERVER] Fall damage of {a_info.DamageToDeal} applied to {m_healthController.OwnerClientId} for a falling speed of {a_fallingSpeed} m/s.");
                    
            OnFallingDamageTaken_ServerCalled?.Invoke(this, damageToDeal);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleFallingDamageTaken_ClientsRpc(int a_fallDamage)
        {
            OnFallingDamageTaken_ClientsCalled?.Invoke(this, a_fallDamage);
        }
    }
}
