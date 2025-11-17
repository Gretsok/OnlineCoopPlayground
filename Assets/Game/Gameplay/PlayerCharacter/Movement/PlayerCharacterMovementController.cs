using System.Collections.Generic;
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
        
        
        private readonly NetworkVariable<NetworkBehaviourReference> m_currentActiveBehaviour = new NetworkVariable<NetworkBehaviourReference>(writePerm: NetworkVariableWritePermission.Server);
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
            m_currentActiveBehaviour.OnValueChanged += HandleCurrentActiveBehaviourChanged;
            if (!IsServer)
            {
                HandleCurrentActiveBehaviourChanged(null, m_currentActiveBehaviour.Value);
            }
        }

        private void HandleCurrentActiveBehaviourChanged(NetworkBehaviourReference a_oldBehaviourReference, NetworkBehaviourReference a_newBehaviourReference)
        {
            a_newBehaviourReference.TryGet(out AMovementBehaviour newBehaviour);
            
            UnityEngine.Debug.Log($"[CLIENT] About to change movement behaviour from" +
                                  $" \"{(CurrentActiveBehaviour ? CurrentActiveBehaviour.gameObject.name : "None")}\" to " +
                                  $" \"{(newBehaviour ? newBehaviour.gameObject.name : "None")}\".");
            
            if (CurrentActiveBehaviour)
                CurrentActiveBehaviour.Deactivate_ForClients();
            CurrentActiveBehaviour = newBehaviour;
            if (CurrentActiveBehaviour)
            {
                CurrentActiveBehaviour.SetDefaultDependencies(Blackboard, m_rigidbody, m_isGroundedController, m_fallingSpeedController);
                CurrentActiveBehaviour.Activate_ForClients();
            }
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            SwitchToBehaviour_ForServer(DefaultMovementBehaviour);
        }

        private readonly List<AMovementBehaviour> m_stackedBehaviours_ServerOnly = new();
        
        private void SwitchToBehaviour_ForServer(AMovementBehaviour a_behaviour)
        {
            if (!IsServer)
                return;
            
            UnityEngine.Debug.Log($"[SERVER] About to change movement behaviour from" +
                                  $" \"{(CurrentActiveBehaviour ? CurrentActiveBehaviour.gameObject.name : "None")}\" to " +
                                  $" \"{(a_behaviour ? a_behaviour.gameObject.name : "None")}\".");
            
            if (CurrentActiveBehaviour)
                CurrentActiveBehaviour.Deactivate_ForServer();
            
            if (a_behaviour)
            {
                a_behaviour.SetDefaultDependencies(Blackboard, m_rigidbody, m_isGroundedController, m_fallingSpeedController);                
                a_behaviour.Activate_ForServer();
            }
            // We change the value after the activate call because it instantly triggers clients callbacks on host.
            m_currentActiveBehaviour.Value = a_behaviour;
            CurrentActiveBehaviour = a_behaviour;

        }

        public void StackBehaviour_ForServer(AMovementBehaviour a_behaviour)
        {
            m_stackedBehaviours_ServerOnly.Add(a_behaviour);
            SwitchToBehaviour_ForServer(a_behaviour);
        }

        public void UnstackBehaviour_ForServer(AMovementBehaviour a_behaviour)
        {
            m_stackedBehaviours_ServerOnly.Remove(a_behaviour);
            
            if (m_stackedBehaviours_ServerOnly.Count > 0)
                SwitchToBehaviour_ForServer(a_behaviour);
            else
                SwitchToBehaviour_ForServer(DefaultMovementBehaviour);
        }
    }
}
