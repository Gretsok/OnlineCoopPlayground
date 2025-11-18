using System.Collections.Generic;
using Game.Gameplay.GameplayInteractionsSystems.SkillSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.SkillsIntegration
{
    public class SkillsInventory : NetworkBehaviour
    {
        [SerializeField]
        private List<ASkill> m_skillsPrefabsToSpawn = new List<ASkill>();

        private readonly NetworkList<NetworkObjectReference> m_instantiatedSkills = new NetworkList<NetworkObjectReference>();
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            if (IsServer)
            {
                InstantiateSkillsPrefabs_Server();
            }
        }

        private void InstantiateSkillsPrefabs_Server()
        {
            for (int i = 0; i < m_skillsPrefabsToSpawn.Count; i++)
            {
                var prefab = m_skillsPrefabsToSpawn[i];

                // We cannot use the NetworkObject reference of prefab since it is only set up once instantiated.
                var newSkill = prefab.GetComponent<NetworkObject>().InstantiateAndSpawn(NetworkManager, OwnerClientId);
                newSkill.TrySetParent(NetworkObject);
                
                m_instantiatedSkills.Add(newSkill);
            }
        }

        public bool HasSkillAtIndex(int a_index)
        {
            if (a_index >= m_instantiatedSkills.Count)
            {
                return false;
            }

            if (a_index < 0)
            {
                return false;
            }
            
            var skillReference = m_instantiatedSkills[a_index];
            return skillReference.TryGet(out NetworkObject networkObject) &&
                   networkObject.TryGetComponent(out ASkill skill);
        }
        
        public ASkill GetSkillByIndex(int a_index)
        {
            if (a_index >= m_instantiatedSkills.Count)
            {
                Debug.LogError($"Cannot retrieve skill: Invalid skill index {a_index}", gameObject);
                return null;
            }
            
            var skillReference = m_instantiatedSkills[a_index];
            if (skillReference.TryGet(out NetworkObject networkObject) &&
                networkObject.TryGetComponent(out ASkill skill))
            {
                return skill;
            }
            else
            {
                Debug.LogError($"Cannot retrieve skill: Invalid skill reference at index {a_index}", gameObject);
                return null;
            }
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            if (!IsServer)
                return;

            for (int i = m_instantiatedSkills.Count - 1; i >= 0; i--)
            {
                var skillReference = m_instantiatedSkills[i];

                if (skillReference.TryGet(out NetworkObject networkObject))
                {
                    Destroy(networkObject.gameObject);
                }
            }
        }
    }
}
