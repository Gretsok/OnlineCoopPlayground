using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.EffectsSystem
{
    /// <summary>
    /// Before making effects from this base class, check if some derived types have more precise ready to use behaviours.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class AEffect : NetworkBehaviour
    {
        public MonoBehaviour ReferencesHolder { get; private set; }

        /// <summary>
        /// Can the effect be played for this gameDataContainer. This should be called from the server.
        /// </summary>
        /// <param name="a_gameDataContainer"></param>
        /// <returns></returns>
        public virtual bool CanBePlayed_ForServer(MonoBehaviour a_gameDataContainer)
        {
            return true;
        }
        
        /// <summary>
        /// Can the effect be played for this gameDataContainer. This should be called from clients. By default, it uses the server implementation.
        /// </summary>
        /// <param name="a_gameDataContainer"></param>
        /// <returns></returns>
        public virtual bool CanBePlayed_ForClient(MonoBehaviour a_gameDataContainer)
        {
            return CanBePlayed_ForServer(a_gameDataContainer);
        }
        
        /// <summary>
        /// Must be called from the server.
        /// </summary>
        /// <param name="a_gameDataContainer"></param>
        public void PlayEffect_ForServer(MonoBehaviour a_gameDataContainer)
        {
            if (!IsServer)
            {
                Debug.LogError($"Only the server can play effects.");
                return;
            }
            if (!CanBePlayed_ForServer(a_gameDataContainer))
            {
                Debug.Log($"The effect cannot be played.");
                return;
            }
            
            ReferencesHolder = a_gameDataContainer;
            HandlePlayEffectCalled_ServerCalled(a_gameDataContainer);
        }

        /// <summary>
        /// Should be called on clients.
        /// </summary>
        /// <param name="a_gameDataContainer"></param>
        public void InjectReferencesHolder_ForClients(MonoBehaviour a_gameDataContainer)
        {
            ReferencesHolder = a_gameDataContainer;
        }

        protected virtual void HandlePlayEffectCalled_ServerCalled(MonoBehaviour a_gameDataContainer)
        {
            
        }
    }
}
