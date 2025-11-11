using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.EffectsSystem
{
    /// <summary>
    /// Instantaneous effects. Overkill will certainly be deleted.
    /// </summary>
    public class InstantEffect : AEffect
    {
        public event Action<InstantEffect> OnEffectPlayed_Server;
        public event Action<InstantEffect> OnEffectPlayed_Clients;

        protected sealed override void HandlePlayEffectCalled_ServerCalled(MonoBehaviour a_gameDataContainer)
        {
            base.HandlePlayEffectCalled_ServerCalled(a_gameDataContainer);
            PlayEffectCalled_ServerCalled();
            PlayEffectCalled_ClientsRpc();
            OnEffectPlayed_Server?.Invoke(this);
        }

        protected virtual void PlayEffectCalled_ServerCalled()
        {
            
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void PlayEffectCalled_ClientsRpc()
        {
            PlayEffectCalled_ClientsCalled();
            OnEffectPlayed_Clients?.Invoke(this);
        }
        
        protected virtual void PlayEffectCalled_ClientsCalled()
        {
            
        }
        
    }
}
