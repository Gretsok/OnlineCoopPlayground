using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.MovementBehaviours
{
    public abstract class AMovementBehaviour : NetworkBehaviour
    {
        protected PlayerCharacterMovementBlackBoard Blackboard { get; private set; }
        protected Rigidbody Rigidbody { get; private set; }
        protected IsGroundedController IsGroundedController { get; private set; }
        protected FallingSpeedController FallingSpeedController { get; private set; }

        private readonly NetworkVariable<bool> m_isActive = new NetworkVariable<bool>(false);
        public bool IsActive => m_isActive?.Value ?? false;
        
        public void SetDefaultDependencies(PlayerCharacterMovementBlackBoard a_blackboard,
            Rigidbody a_rigidbody, 
            IsGroundedController a_isGroundedController,
            FallingSpeedController a_fallingSpeedController)
        {
            Blackboard = a_blackboard;
            Rigidbody = a_rigidbody;
            IsGroundedController = a_isGroundedController;
            FallingSpeedController = a_fallingSpeedController;
        }

        public void Activate_ForServer()
        {
            if (!IsServer)
                return;
            
            Blackboard.OnBlockActivated_ServerCalled += HandleBlockActivated_ServerCalled;
            Blackboard.OnBlockDeactivated_ServerCalled += HandleBlockDeactivated_ServerCalled;
            
            m_isActive.Value = true;
            
            HandleActivation_ServerCalled();
        }

        public void Activate_ForClients()
        {
            if (!IsOwner)
                return;
            
            Blackboard.OnBlockActivated_OwnerCalled += HandleBlockActivated_OwnerCalled;
            Blackboard.OnBlockDeactivated_OwnerCalled += HandleBlockDeactivated_OwnerCalled;
            
            Blackboard.OnJumpStarted_OwnerCalled += HandleJumpStarted_OwnerCalled;
            Blackboard.OnJumpStopped_OwnerCalled += HandleJumpStopped_OwnerCalled;
            
            HandleActivation_OwnerCalled();
        }
        
        protected virtual void HandleActivation_ServerCalled() {}
        protected virtual void HandleActivation_OwnerCalled() {}

        public void Deactivate_ForServer()
        {
            if (!IsServer)
                return;
            
            Blackboard.OnBlockActivated_ServerCalled -= HandleBlockActivated_ServerCalled;
            Blackboard.OnBlockDeactivated_ServerCalled -= HandleBlockDeactivated_ServerCalled;

            m_isActive.Value = false;
            
            HandleDeactivation_ServerCalled();
        }

        public void Deactivate_ForClients()
        {
            if (!IsOwner)
                return;

            Blackboard.OnBlockActivated_OwnerCalled -= HandleBlockActivated_OwnerCalled;
            Blackboard.OnBlockDeactivated_OwnerCalled -= HandleBlockDeactivated_OwnerCalled;

            Blackboard.OnJumpStarted_OwnerCalled -= HandleJumpStarted_OwnerCalled;
            Blackboard.OnJumpStopped_OwnerCalled -= HandleJumpStopped_OwnerCalled;
            
            HandleDeactivation_OwnerCalled();
        }
        
        protected virtual void HandleDeactivation_ServerCalled() {}
        protected virtual void HandleDeactivation_OwnerCalled() {}


        protected virtual void HandleBlockActivated_OwnerCalled(PlayerCharacterMovementBlackBoard a_blackboard)
        {
            
        }

        protected virtual void HandleBlockActivated_ServerCalled(PlayerCharacterMovementBlackBoard a_blackboard)
        {
            
        }

        protected virtual void HandleBlockDeactivated_OwnerCalled(PlayerCharacterMovementBlackBoard a_blackboard)
        {
            
        }

        protected virtual void HandleBlockDeactivated_ServerCalled(PlayerCharacterMovementBlackBoard a_blackboard)
        {
            
        }
        
        
        protected virtual void HandleJumpStarted_OwnerCalled(PlayerCharacterMovementBlackBoard a_obj)
        {
            
        }

        protected virtual void HandleJumpStopped_OwnerCalled(PlayerCharacterMovementBlackBoard a_obj)
        {
            
        }

        private void FixedUpdate()
        {
            if (!IsActive)
                return;

            if (IsServer)
            {
                FixedUpdateAsServer();
            }

            if (IsClient)
            {
                FixedUpdateAsClient();
            }

            if (IsOwner)
            {
                FixedUpdateAsOwner();
            }
        }

        protected virtual void FixedUpdateAsServer()
        {}
        
        protected virtual void FixedUpdateAsClient()
        {}
        
        protected virtual void FixedUpdateAsOwner()
        {}
    }
}