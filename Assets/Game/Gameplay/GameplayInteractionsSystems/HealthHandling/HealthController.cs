using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.HealthHandling
{
    public class HealthController : NetworkBehaviour
    {
        [SerializeField]
        private int m_startingMaxHealth = 100;
        private readonly NetworkVariable<int> m_maxHealth = new NetworkVariable<int>(-1);
        public int MaxHealth => m_maxHealth?.Value ?? -1;
        
        private readonly NetworkVariable<int> m_currentHealth = new NetworkVariable<int>(-1);
        public int CurrentHealth => m_currentHealth?.Value ?? -1;

        public event Action<HealthController, int> OnDamageTaken_ServerCalled;
        public event Action<HealthController, int> OnDamageTaken_ClientsCalled;
        
        public event Action<HealthController> OnDeath_ServerCalled;
        public event Action<HealthController> OnDeath_ClientsCalled;
        
        public event Action<HealthController, int> OnHealReceived_ServerCalled;
        public event Action<HealthController, int> OnHealReceived_ClientsCalled;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
                return;
            
            m_maxHealth.Value = m_startingMaxHealth;
            m_currentHealth.Value = m_maxHealth.Value;
        }


        public void TakeDamage_ForServer(int a_damageToTake)
        {
            if (!IsServer)
            {
                Debug.LogError($"Only server can deal damages.");
                return;
            }
            
            m_currentHealth.Value -= a_damageToTake;
            OnDamageTaken_ServerCalled?.Invoke(this, a_damageToTake);
            HandleDamageTaken_ClientsRpc(a_damageToTake);

            if (m_currentHealth.Value <= 0)
            {
                OnDeath_ServerCalled?.Invoke(this);
                HandleDeath_ClientsRpc();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleDamageTaken_ClientsRpc(int a_damageDealt)
        {
            OnDamageTaken_ClientsCalled?.Invoke(this, a_damageDealt);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleDeath_ClientsRpc()
        {
            OnDeath_ClientsCalled?.Invoke(this);
        }
        

        
        public void Heal_ForServer(int a_amountToHeal)
        {
            if (!IsServer)
            {
                Debug.LogError($"Only server can heal.");
                return;
            }
            
            m_currentHealth.Value += a_amountToHeal;
            
            OnHealReceived_ServerCalled?.Invoke(this, a_amountToHeal);
            HandleHeal_ClientsRpc(a_amountToHeal);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleHeal_ClientsRpc(int a_amountToHeal)
        {
            OnHealReceived_ClientsCalled?.Invoke(this, a_amountToHeal);
        }
    }
}
