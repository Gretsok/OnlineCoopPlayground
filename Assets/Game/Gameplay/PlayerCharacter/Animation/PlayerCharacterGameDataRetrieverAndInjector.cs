using Game.Gameplay.PlayerCharacter.Movement;
using Tools.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Animation
{
    public class PlayerCharacterGameDataRetrieverAndInjector : NetworkBehaviour
    {
        private MonoBehaviour m_parent;

        [SerializeField]
        private PlayerCharacterAnimationController m_animationController;

        public void SetDependencies(MonoBehaviour a_parent)
        {
            m_parent = a_parent;
        }
        
        private void Update()
        {
            if (m_parent is IPlayerCharacterMovementControllerHolder movementControllerHolder &&
                movementControllerHolder.MovementController.CurrentActiveBehaviour ==
                movementControllerHolder.MovementController.DefaultMovementBehaviour)
            {
                var flattenVelocity = movementControllerHolder.MovementController.DefaultMovementBehaviour.CurrentPlanarVelocity.Flatten();
                m_animationController.SetForwardSpeed(flattenVelocity.magnitude);

                if (flattenVelocity.sqrMagnitude > 0.3f * 0.3f)
                {
                    m_parent.transform.forward = Vector3.Slerp(m_parent.transform.forward,
                        flattenVelocity.normalized, Time.deltaTime * 18f);
                }
            }

            if (m_parent is IIsGroundedControllerHolder isGroundedControllerHolder)
                m_animationController.SetIsGrounded(isGroundedControllerHolder.IsGroundedController.IsGrounded);
        }
    }
}
