using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.EffectsSystem
{
    /// <summary>
    /// Effect that last a certain period in time.
    /// </summary>
    public class TimedEffect : AEffect
    {
        [SerializeField]
        private float m_baseEffectDuration;
        public float BaseEffectDuration => m_baseEffectDuration;
        public virtual float EffectDuration => BaseEffectDuration;

        private readonly NetworkVariable<int> m_startTick = new NetworkVariable<int>();
        public int StartTick => m_startTick.Value;
        
        public event Action<TimedEffect> OnTimedEffectStarted_ServerCalled;
        public event Action<TimedEffect> OnTimedEffectStarted_ClientsCalled;
        public event Action<TimedEffect> OnTimedEffectEnded_ServerCalled;
        public event Action<TimedEffect> OnTimedEffectEnded_ClientsCalled;
        
        protected sealed override void HandlePlayEffectCalled_ServerCalled(MonoBehaviour a_gameDataContainer)
        {
            base.HandlePlayEffectCalled_ServerCalled(a_gameDataContainer);

            m_startTick.Value = NetworkManager.ServerTime.Tick;
            HandleActiveEffectStarted_ServerCalled();
            HandleActiveEffectStarted_ClientsRpc();
            OnTimedEffectStarted_ServerCalled?.Invoke(this);
            NetworkManager.NetworkTickSystem.Tick += HandleTickedMasterLogic_Server;
        }

        private void HandleTickedMasterLogic_Server()
        {
            var currentTick = NetworkManager.ServerTime.Tick;
            var deltaTickCount = currentTick - m_startTick.Value;
            var deltaTimeSinceStartInTickTime = deltaTickCount * NetworkManager.ServerTime.FixedDeltaTime;
            if (deltaTimeSinceStartInTickTime >= EffectDuration)
            {
                NotifyEndOfEffect_ServerCalled();
            }
            else
            {
                HandleTick_ServerCalled();
            }
        }

        #region Server Callbacks
        /// <summary>
        /// Derive from it to handle logic on the server when the effect has started.
        /// </summary>
        protected virtual void HandleActiveEffectStarted_ServerCalled()
        {
            
        }

        /// <summary>
        /// Derive from it to handle logic on the server using the <see cref="NetworkTickSystem"/> fixed rhythm.
        /// </summary>
        protected virtual void HandleTick_ServerCalled()
        {
            
        }
        
        /// <summary>
        /// Derive from it to handle logic on the server when the effect has started.
        /// </summary>
        protected virtual void HandleActiveEffectEnded_ServerCalled()
        {
            
        }
        #endregion

        private void NotifyEndOfEffect_ServerCalled()
        {
            NetworkManager.NetworkTickSystem.Tick -= HandleTickedMasterLogic_Server;

            HandleActiveEffectEnded_ServerCalled();
            
            OnTimedEffectEnded_ServerCalled?.Invoke(this);
            HandleActiveEffectEnded_ClientsRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleActiveEffectStarted_ClientsRpc()
        {
            NetworkManager.NetworkTickSystem.Tick -= HandleTick_ClientsCalled;
            NetworkManager.NetworkTickSystem.Tick += HandleTick_ClientsCalled;
            HandleActiveEffectStarted_ClientsCalled();
            OnTimedEffectStarted_ClientsCalled?.Invoke(this);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void HandleActiveEffectEnded_ClientsRpc()
        {
            NetworkManager.NetworkTickSystem.Tick -= HandleTick_ClientsCalled;
            HandleActiveEffectEnded_ClientsCalled();
            OnTimedEffectEnded_ClientsCalled?.Invoke(this);
        }

        private void Update()
        {
            if (!IsClient)
                return;
            
            HandleUpdate_ClientsCalled();
        }

        #region Clients Callbacks
        /// <summary>
        /// Derive from it to handle logic on clients when the effect has started.
        /// </summary>
        protected virtual void HandleActiveEffectStarted_ClientsCalled()
        {

        }
        
        /// <summary>
        /// Derive from it to handle logic on clients using the <see cref="NetworkTickSystem"/> fixed rhythm.
        /// </summary>
        protected virtual void HandleTick_ClientsCalled()
        {
            
        }

        /// <summary>
        /// Derive from it to handle logic on clients in the Update method.
        /// </summary>
        protected virtual void HandleUpdate_ClientsCalled()
        {
            
        }
        
        /// <summary>
        /// Derive from it to handle logic on clients when the effect has started.
        /// </summary>
        protected virtual void HandleActiveEffectEnded_ClientsCalled()
        {
            
        }
        #endregion
    }
}
