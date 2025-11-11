using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.SkillSystem
{
    [RequireComponent(typeof(NetworkObject))]
    public class ASkill : NetworkBehaviour
    {
        private readonly NetworkVariable<bool> m_isPerforming = new NetworkVariable<bool>();
        public bool IsPerforming => m_isPerforming?.Value ?? false;

        public event Action<ASkill, AReferencesHolderForSkills> OnSkillTriggered_ServerCalled;
        public event Action<ASkill, AReferencesHolderForSkills> OnSkillTriggered_ClientsCalled;
        public event Action<ASkill, AReferencesHolderForSkills> OnSkillCancelled_ServerCalled;
        public event Action<ASkill, AReferencesHolderForSkills> OnSkillCancelled_ClientsCalled;
        public event Action<ASkill, AReferencesHolderForSkills> OnSkillStopped_ServerCalled;
        public event Action<ASkill, AReferencesHolderForSkills> OnSkillStopped_ClientsCalled;

        private AReferencesHolderForSkills m_referencesHolderForSkills;
        
        public void InjectReferencesHolder_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            m_referencesHolderForSkills = a_referencesHolderForSkills;
        }
        
        public bool CanBeTriggered_ForServer()
        {
            return !IsPerforming;
        }

        public bool CanBeTriggered_ForClients()
        {
            return CanBeTriggered_ForServer();
        }
        
        public bool TriggerSkill_ForServer(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            if (!IsServer)
            {
                Debug.LogError($"Only server can trigger skills.", gameObject);
                return false;
            }

            if (!CanBeTriggered_ForServer())
            {
                Debug.Log($"Cannot trigger the skill {gameObject.name}.", gameObject);
                return false;
            }
            
            m_referencesHolderForSkills = a_referencesHolderForSkills;
            
            m_isPerforming.Value = true;
            HandleSkillTriggered_ServerCalled(a_referencesHolderForSkills);
            OnSkillTriggered_ServerCalled?.Invoke(this, a_referencesHolderForSkills);
            HandleSkillTriggered_ClientsRpc();
            return true;
        }
        
        protected virtual void HandleSkillTriggered_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }

        private void HandleSkillTriggered_ClientsRpc()
        {
            HandleSkillTriggered_ClientsCalled(m_referencesHolderForSkills);  
            OnSkillTriggered_ClientsCalled?.Invoke(this, m_referencesHolderForSkills);
        }
        
        protected virtual void HandleSkillTriggered_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }
        
        
        public bool CanBeCancelled_ForServer(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            return IsPerforming;
        }
        
        public bool CanBeCancelled_ForClients(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            return CanBeCancelled_ForServer(a_referencesHolderForSkills);
        }
        
        public void CancelTriggerSkill_ForServer(AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            if (!IsServer)
            {
                Debug.LogError($"Only server can cancel skills.", gameObject);
                return;
            }
            if (IsPerforming)
            {
                Debug.LogError($"Cannot cancel a skill that is not performing. How did that happen?", gameObject);
                return;
            }
            if (!CanBeCancelled_ForServer(a_referencesHolderForSkills))
            {
                Debug.Log($"Cannot cancel the skill {gameObject.name}.", gameObject);
                return;
            }
            
            HandleSkillPreCanceled_ServerCalled(a_referencesHolderForSkills);
            StopSkill_ForServer();
            HandleSkillPostCanceled_ClientsRpc();
            HandleSkillPostCanceled_ServerCalled(a_referencesHolderForSkills);
            OnSkillCancelled_ServerCalled?.Invoke(this, a_referencesHolderForSkills);
        }
        
        protected virtual void HandleSkillPreCanceled_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }
        
        protected virtual void HandleSkillPostCanceled_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleSkillPostCanceled_ClientsRpc()
        {
            HandleSkillPostCanceled_ClientsCalled(m_referencesHolderForSkills);
            OnSkillCancelled_ClientsCalled?.Invoke(this, m_referencesHolderForSkills);
        }
        
        protected virtual void HandleSkillPostCanceled_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }
        
        protected void StopSkill_ForServer()
        {
            if (!IsServer)
            {
                Debug.LogError($"Only server can stop skills.", gameObject);
                return;
            }
            
            m_isPerforming.Value = false;
            HandleSkillStopped_ServerCalled(m_referencesHolderForSkills);
            OnSkillStopped_ServerCalled?.Invoke(this, m_referencesHolderForSkills);
            HandleSkillStopped_ClientsRpc();
        }

        protected virtual void HandleSkillStopped_ServerCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }

        [Rpc(SendTo.ClientsAndHost)]
        private void HandleSkillStopped_ClientsRpc()
        {
            HandleSkillStopped_ClientsCalled(m_referencesHolderForSkills);  
            OnSkillStopped_ClientsCalled?.Invoke(this, m_referencesHolderForSkills);
        }
        
        protected virtual void HandleSkillStopped_ClientsCalled(AReferencesHolderForSkills a_referencesHolderForSkills)
        { }
    }
}
