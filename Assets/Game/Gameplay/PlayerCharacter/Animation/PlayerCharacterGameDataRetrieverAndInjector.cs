using Tools.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Animation
{
    public class PlayerCharacterGameDataRetrieverAndInjector : NetworkBehaviour
    {
        [SerializeField]
        private PlayerCharacter m_playerCharacter;

        [SerializeField]
        private PlayerCharacterAnimationController m_animationController;
        
        private void Update()
        {
            if (m_playerCharacter.MovementController.CurrentActiveBehaviour ==
                m_playerCharacter.MovementController.DefaultMovementBehaviour)
            {
                var flattenVelocity = m_playerCharacter.MovementController.DefaultMovementBehaviour.CurrentPlanarVelocity.Flatten();
                m_animationController.SetForwardSpeed(flattenVelocity.magnitude);

                if (flattenVelocity.sqrMagnitude > 0.3f * 0.3f)
                {
                    m_playerCharacter.transform.forward = Vector3.Slerp(m_playerCharacter.transform.forward,
                        flattenVelocity.normalized, Time.deltaTime * 18f);
                }
            }

            m_animationController.SetIsGrounded(m_playerCharacter.IsGroundedController.IsGrounded);
        }
    }
}
