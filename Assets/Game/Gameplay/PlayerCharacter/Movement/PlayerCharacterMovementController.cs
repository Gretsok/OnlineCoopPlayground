using Game.Gameplay.PlayerCharacter.Movement.MovementBehaviours;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public class PlayerCharacterMovementController : NetworkBehaviour
    {
        [field: SerializeField]
        public PlayerCharacterMovementBlackBoard Blackboard { get; private set; }
        
        [field: SerializeField]
        public DefaultMovementBehaviour DefaultMovementBehaviour { get; private set; }
        
        
        private readonly NetworkVariable<ushort> m_currentActiveBehaviourOrderIndex = new NetworkVariable<ushort>(writePerm: NetworkVariableWritePermission.Server);
        public AMovementBehaviour CurrentActiveBehaviour { get; private set; }
        
        private Rigidbody m_rigidbody;
        private IsGroundedController m_isGroundedController;
        private FallingSpeedController m_fallingSpeedController;
        
        public void SetDependencies(Rigidbody a_rigidbody, FallingSpeedController a_fallingSpeedController, IsGroundedController a_isGroundedController)
        {
            m_rigidbody = a_rigidbody;
            m_isGroundedController = a_isGroundedController;
            m_fallingSpeedController = a_fallingSpeedController;
            
            DefaultMovementBehaviour.SetDefaultDependencies(Blackboard, a_rigidbody, a_isGroundedController, a_fallingSpeedController);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_currentActiveBehaviourOrderIndex.OnValueChanged += HandleCurrentActiveBehaviourChanged;
            if (!IsServer)
            {
                HandleCurrentActiveBehaviourChanged(0, m_currentActiveBehaviourOrderIndex.Value);
            }
        }

        private void HandleCurrentActiveBehaviourChanged(ushort a_previousValue, ushort a_newValue)
        {
            if (CurrentActiveBehaviour)
                CurrentActiveBehaviour.Deactivate_ForClients();
            CurrentActiveBehaviour = NetworkObject.GetNetworkBehaviourAtOrderIndex(a_newValue) as AMovementBehaviour;
            if (CurrentActiveBehaviour)
                CurrentActiveBehaviour.Activate_ForClients();
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            SwitchToBehaviour_ForServer(DefaultMovementBehaviour);
        }

        public void SwitchToBehaviour_ForServer(AMovementBehaviour a_behaviour)
        {
            if (!IsServer)
                return;
            
            if (CurrentActiveBehaviour)
                CurrentActiveBehaviour.Deactivate_ForServer();
            m_currentActiveBehaviourOrderIndex.Value = NetworkObject.GetNetworkBehaviourOrderIndex(a_behaviour);
            if (a_behaviour)
                a_behaviour.Activate_ForServer();
        }
    }
}
