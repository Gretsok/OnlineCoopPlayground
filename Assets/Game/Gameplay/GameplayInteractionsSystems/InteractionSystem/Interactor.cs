using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem
{
    public class Interactor : NetworkBehaviour
    {
        [SerializeField]
        private InteractablesDetector m_interactablesDetector;
        
        public MonoBehaviour Source { get; private set; }

        public void SetSource(MonoBehaviour a_source)
        {
            Source = a_source;
            m_interactablesDetector.SetSource(a_source.transform);
        }
        
        public Interactable InteractableToInteractWith { get; private set; }

        private void FixedUpdate()
        {
            if (!IsOwner) 
                return;
            
            var interactableToInteractWith = m_interactablesDetector.GetInteractableToInteractWith();

            if (interactableToInteractWith != InteractableToInteractWith)
            {
                if (InteractableToInteractWith)
                    InteractableToInteractWith.NotifySightOfLocalCharacterLeft_ForClients(this);
                InteractableToInteractWith = interactableToInteractWith;
                if (InteractableToInteractWith)
                    InteractableToInteractWith.NotifySightOfLocalCharacterEntered_ForClients(this);
            }
        }

        public void TryToInteract_ForOwner()
        {
            if (!IsOwner)
                return;
            
            if (!InteractableToInteractWith)
            {
                Debug.Log("No interactable to interact with.");
                return;
            }
            
            TryToInteract_ServerRpc(InteractableToInteractWith.NetworkObject,
                InteractableToInteractWith.NetworkObject.GetNetworkBehaviourOrderIndex(InteractableToInteractWith));
        }

        [Rpc(SendTo.Server)]
        private void TryToInteract_ServerRpc(NetworkObjectReference a_networkObject, ushort a_componentOrderIndex)
        {
            if (!IsServer)
                return;

            if (!a_networkObject.TryGet(out NetworkObject networkObject))
            {
                Debug.LogError($"Network object of interactable to interact with is not valid.");
                return;
            }
            
            InteractableToInteractWith = networkObject.GetNetworkBehaviourAtOrderIndex(a_componentOrderIndex) as Interactable;
            
            if (!InteractableToInteractWith)
            {
                Debug.Log("Interactable to interact with is not valid.");
                return;
            }
            
            InteractableToInteractWith.RequestInteraction_ForServer(this);
            
            InteractableToInteractWith = null;
        }
    }
}
