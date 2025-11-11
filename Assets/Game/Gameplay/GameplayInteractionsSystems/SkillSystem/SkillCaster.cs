using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.SkillSystem
{
    public class SkillCaster : NetworkBehaviour
    {
        [SerializeField]
        private AReferencesHolderForSkills m_referencesHolderForSkills;
        
        private readonly NetworkVariable<bool> m_isPerformingASkill = new NetworkVariable<bool>();
        public bool IsPerformingASkill => m_isPerformingASkill?.Value ?? false;
            
        /// <summary>
        /// Trigger a skill.
        /// </summary>
        /// <param name="a_skill">The skill must already be instantiated and spawned over the network.</param>
        public void TryToTriggerSkill_ForOwner(ASkill a_skill)
        {
            if (!IsOwner)
            {
                Debug.LogError($"You must be the owner of this skill to try to trigger it.", a_skill.gameObject);
                return;
            }
            
            TriggerSkill_ServerRpc(a_skill.NetworkObject);
        }

        [Rpc(SendTo.Server)]
        private void TriggerSkill_ServerRpc(NetworkObjectReference a_skillReference)
        {
            if (!IsServer)
            {
                Debug.LogError($"You must be the server to play a skill.", gameObject);
                return;
            }

            if (a_skillReference.TryGet(out NetworkObject networkObject) &&
                networkObject.TryGetComponent(out ASkill skill))
            {
                skill.OnSkillStopped_ServerCalled += HandleSkillStopped_ServerCalled;
                SendReferencesHolderToClients_ClientsRpc(networkObject);
                if (!skill.TriggerSkill_ForServer(m_referencesHolderForSkills))
                {
                    skill.OnSkillStopped_ServerCalled -= HandleSkillStopped_ServerCalled;
                }
                else
                {
                    m_isPerformingASkill.Value = true;
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendReferencesHolderToClients_ClientsRpc(NetworkObjectReference a_skillReference)
        {
            if (a_skillReference.TryGet(out NetworkObject networkObject) &&
                networkObject.TryGetComponent(out ASkill skill))
            {
                skill.InjectReferencesHolder_ClientsCalled(m_referencesHolderForSkills);
            }
        }

        private void HandleSkillStopped_ServerCalled(ASkill a_skill, AReferencesHolderForSkills a_referencesHolderForSkills)
        {
            a_skill.OnSkillStopped_ServerCalled -= HandleSkillStopped_ServerCalled;
            
            m_isPerformingASkill.Value = false;
        }
    }
}
