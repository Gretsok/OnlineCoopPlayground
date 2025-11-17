using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem
{
    public class Interactable : NetworkBehaviour, IInteractableHolder
    {
        public delegate bool SCondition(Interactor a_interactor);
        
        private readonly List<SCondition> m_serverConditions = new List<SCondition>();

        public void AddCondition(SCondition a_condition)
        {
            if (!m_serverConditions.Contains(a_condition))
                m_serverConditions.Add(a_condition);
        }

        public void RemoveCondition(SCondition a_condition)
        {
            m_serverConditions.RemoveAll(a_existingCondition => a_existingCondition == a_condition);
        }
        
        public virtual bool CanInteractWith_ForServer(Interactor a_interactor)
        {
            if (!a_interactor)
                return false;
            
            for (int i = 0; i < m_serverConditions.Count; i++)
            {
                var condition = m_serverConditions[i];
                
                if (!condition(a_interactor))
                    return false;
            }

            return true;
        }
        
        public event Action<Interactable, Interactor> OnSightOfLocalCharacterEntered_ClientsCalled;
        [SerializeField]
        private UnityEvent m_onSightOfLocalCharacterEntered_ClientsCalled;
        
        public void NotifySightOfLocalCharacterEntered_ForClients(Interactor a_interactor)
        {
            OnSightOfLocalCharacterEntered_ClientsCalled?.Invoke(this, a_interactor);
            m_onSightOfLocalCharacterEntered_ClientsCalled?.Invoke();
        }
        
        public event Action<Interactable, Interactor> OnSightOfLocalCharacterLeft_ClientsCalled;
        [SerializeField]
        private UnityEvent m_onSightOfLocalCharacterLeft_ClientsCalled;
        public void NotifySightOfLocalCharacterLeft_ForClients(Interactor a_interactor)
        {
            OnSightOfLocalCharacterLeft_ClientsCalled?.Invoke(this, a_interactor);
            m_onSightOfLocalCharacterLeft_ClientsCalled?.Invoke();
        }

        
        public event Action<Interactable, Interactor> OnInteractionRequested_ServerCalled;

        [SerializeField]
        private UnityEvent m_onInteractionRequested_ServerCalled;
        public event Action<Interactable, Interactor> OnInteractionRequested_ClientsCalled;
        [SerializeField]
        private UnityEvent m_onInteractionRequested_ClientsCalled;
        public void RequestInteraction_ForServer(Interactor a_interactor)
        {
            if (!IsServer)
                return;
            
            OnInteractionRequested_ServerCalled?.Invoke(this, a_interactor);
            m_onInteractionRequested_ServerCalled?.Invoke();
            HandleInteractionRequested_ClientsRpc(a_interactor.NetworkObject, a_interactor.NetworkObject.GetNetworkBehaviourOrderIndex(a_interactor));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleInteractionRequested_ClientsRpc(NetworkObjectReference a_interactorReference, ushort a_componentOrderIndex)
        {
            if (a_interactorReference.TryGet(out NetworkObject netObj))
            {
                var interactor = netObj.GetNetworkBehaviourAtOrderIndex(a_componentOrderIndex) as Interactor;
                if (!interactor)
                    return;
                
                OnInteractionRequested_ClientsCalled?.Invoke(this, interactor);
                m_onInteractionRequested_ClientsCalled?.Invoke();
            }
        }

        public Interactable LinkedInteractable => this;
    }
}
