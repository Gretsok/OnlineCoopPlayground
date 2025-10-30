using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.SkillSystem
{
    public class ASkill : NetworkBehaviour
    {

        private readonly NetworkVariable<bool> m_isBeingTriggered = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
        public bool IsBeingTriggered => m_isBeingTriggered?.Value ?? false;
        
        public void TriggerSkill()
        {
            if (IsBeingTriggered)
                return;
            if (!IsOwner)
                return;

        }

        [Rpc(SendTo.Server)]
        private void TriggerSkill_Internal_Rpc()
        {
            
        }

        public void StopTriggerSkill()
        {
            if (!IsBeingTriggered)
                return;
            
            
        }

        [Rpc(SendTo.Server)]
        private void StopTriggeringSkill_Internal_Rpc()
        {
            
        }
    }
}
