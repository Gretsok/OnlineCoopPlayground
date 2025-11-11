using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.EffectsSystem
{
    public class EffectsController : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour m_referencesHolder;
        
        private List<TimedEffect> m_onGoingTimeEffects = new List<TimedEffect>();

        public void AddTimedEffect_ForServer(TimedEffect a_timedEffect)
        {
            if (!a_timedEffect.CanBePlayed_ForServer(m_referencesHolder))
                return;

            if (!a_timedEffect.IsSpawned)
            {
                a_timedEffect = Instantiate(a_timedEffect, NetworkObject.transform);
                a_timedEffect.NetworkObject.Spawn();
            }
            a_timedEffect.NetworkObject.TrySetParent(NetworkObject);
            SendReferencesHolderToTimedEffect_ClientsRpc(a_timedEffect.NetworkObject);
            
            a_timedEffect.OnTimedEffectEnded_ServerCalled += _ =>
            {
                Destroy(a_timedEffect.gameObject, 2f);
            };
            
            a_timedEffect.PlayEffect_ForServer(m_referencesHolder);
            
            
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendReferencesHolderToTimedEffect_ClientsRpc(NetworkObjectReference a_effectReference)
        {
            if (a_effectReference.TryGet(out NetworkObject netObject) &&
                netObject.TryGetComponent(out TimedEffect timeEffect))
            {
                timeEffect.InjectReferencesHolder_ForClients(m_referencesHolder);
            }
            else
            {
                Debug.LogError($"Cannot send references holder to timed effect : Could not recover timed effect.");
            }
        }

        /// <summary>
        /// Overkill. Will be deleted.
        /// </summary>
        /// <param name="a_instantEffect"></param>
        public void PlayInstantEffect_ForServer(InstantEffect a_instantEffect)
        {
            if (!a_instantEffect.CanBePlayed_ForServer(m_referencesHolder))
                return;

            if (!a_instantEffect.IsSpawned)
            {
                a_instantEffect = Instantiate(a_instantEffect, NetworkObject.transform);
                a_instantEffect.NetworkObject.Spawn();
            }
            
            a_instantEffect.NetworkObject.TrySetParent(NetworkObject);
            SendReferencesHolderToTimedEffect_ClientsRpc(a_instantEffect.NetworkObject);
            a_instantEffect.PlayEffect_ForServer(m_referencesHolder);
            Destroy(a_instantEffect.NetworkObject.gameObject, 5f);
        }
    }
}
